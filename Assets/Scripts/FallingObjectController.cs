using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MainSceneEnumList;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;

public class FallingObjectController : SingletonMonoBehaviour<FallingObjectController>
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private GameObject fallingObjectPrefab;
    [SerializeField] private GameObject fallingObjectParent;
    [SerializeField] private List<float> objcetScaleByTypeList = new();
    [SerializeField] private List<AssetReferenceSprite> spriteRefList = new();
    [SerializeField] private float generationIntervalTime;
    [SerializeField] private float dropIntervalTime;
    [SerializeField] private float ignoreGameoverJudgmentTime;
    [SerializeField] private GameObject lineCanDropObject;

    //値のキャッシュ用変数
    [Header("デバック用")]
    [SerializeField] private List<Sprite> spriteList = new();
    [SerializeField] private GameObject holdingFallingObject;
    private List<Vector3> spriteScaleList = new();
    private Transform fallingObjectParentTransform;
    private List<GameObject> fallingObjectList = new();
    private List<StatusFallingObject> statusFallingObjectList = new();
    private List<SpriteRenderer> fallingObjectSpriteRendererList = new();
    private int numberOfObjectType;
    private string tagOfFallingObject;
    private string tagOfFallingObjectIgnoreGameover;
    private string tagOfFallingObjectBeforeDrop;
    private MainSceneController mainSceneController;
    private ScoreController scoreController;
    private (float minX, float maxX) rangeXPositionCanDropObject;
    private float yPositionCanDropObject;
    [SerializeField] private float generationIntervalTimer;
    [SerializeField] private float dropIntervalTimer;

    public async UniTask Initialization()
    {
        string errorMassage = "インスペクター未記入";
        if (fallingObjectPrefab == null || fallingObjectParent == null || objcetScaleByTypeList.Count == 0 || spriteRefList.Count == 0 || generationIntervalTime == 0) Debug.LogError(errorMassage);
        //設定したスプライトを順に読み込み
        for (int i = 0; i < spriteRefList.Count; i++)
        {
            //FallingObject用のスプライトをロード
            var loadSprite = await Addressables.LoadAssetAsync<Sprite>(spriteRefList[i]);
            spriteList.Add(loadSprite);
            //スプライトの大きさを取得し、FallingObjectにスプライトをアタッチした際に指定した大きさにできる拡大倍率を計算
            float scaleX = objcetScaleByTypeList[i] / spriteList[i].bounds.size.x;
            float scaleY = objcetScaleByTypeList[i] / spriteList[i].bounds.size.y;
            spriteScaleList.Add(new Vector3(scaleX, scaleY, 1.0f));
        }
        //メンバ変数の初期化
        numberOfObjectType = objcetScaleByTypeList.Count;
        fallingObjectParentTransform = fallingObjectParent.transform;
        generationIntervalTimer = generationIntervalTime;
        dropIntervalTimer = dropIntervalTime;
        mainSceneController = MainSceneController.Instance;
        scoreController = ScoreController.Instance;
        tagOfFallingObjectIgnoreGameover = GameObjectTag.FallingObjectIgnoreGameover.ToString();
        tagOfFallingObject = GameObjectTag.FallingObject.ToString();
        tagOfFallingObjectBeforeDrop = GameObjectTag.FallingObjectBeforeDrop.ToString();
        //オブジェクトを落とせる位置を計算
        float sizeX = lineCanDropObject.GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 position = lineCanDropObject.transform.position;
        rangeXPositionCanDropObject = (position.x - (sizeX / 2), position.x + (sizeX / 2));
        yPositionCanDropObject = position.y;
        //メンバ変数の初期化続き
        holdingFallingObject = null;
        //Tmpオブジェクトを削除
        Destroy(GameObject.Find("Tmp"));
    }

    //毎フレーム呼び出すのでメンバ変数として定義
    private Vector3 mousePosition;
    private Vector3 holdingObjectPosition;
    private PolygonCollider2D holdingObjectCollider;
    private Rigidbody2D holdingObjectRigitbody2D;
    public void UpdateMe()
    {
        if (holdingFallingObject == null)
        {
            if (generationIntervalTimer > 0) generationIntervalTimer--;
            if (generationIntervalTimer != 0) return;
            holdingFallingObject = GenerateFallingObject();
            generationIntervalTimer = generationIntervalTime;
        }
        //オブジェクトの当たり判定を無効状態にしておく
        if (holdingObjectCollider == null) holdingObjectCollider = holdingFallingObject.GetComponent<PolygonCollider2D>();
        if (holdingObjectCollider.enabled) holdingObjectCollider.enabled = false;
        //物理演算も停止しておく
        if (holdingObjectRigitbody2D == null) holdingObjectRigitbody2D = holdingFallingObject.GetComponent<Rigidbody2D>();
        holdingObjectRigitbody2D.isKinematic = true;
        //マウスの位置を元にオブジェクトの位置を更新
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; //z座標の取得値はカメラと同じ、カメラに映るようにz座標を変更
        //オブジェクトのx座標が落下可能な範囲を超えない様に補正
        if (mousePosition.x < rangeXPositionCanDropObject.minX)
            holdingObjectPosition.x = rangeXPositionCanDropObject.minX;
        else if (mousePosition.x > rangeXPositionCanDropObject.maxX)
            holdingObjectPosition.x = rangeXPositionCanDropObject.maxX;
        else
            holdingObjectPosition.x = mousePosition.x;
        //オブジェクトのy座標は固定
        holdingObjectPosition.y = yPositionCanDropObject;
        holdingFallingObject.transform.position = holdingObjectPosition;
        //オブジェクトを落とす処理
        if (dropIntervalTimer > 0) dropIntervalTimer--;
        if (dropIntervalTimer != 0) return;
        if (Input.GetMouseButtonUp(0))
        {
            //タグを更新、一定時間のみGameover判定を無視されるタグとする
            //FallingObjectBeforeDrop -> FallingObjectIgnoreGameover -> FallingObject
            _ = TemporarilyIgnoreGameover(holdingFallingObject); //awaitしないで処理を走らせておく
            //オブジェクト投下時にタイプに応じたスコアを加算
            scoreController.ScoreWhenDropFallingObject(holdingFallingObject.GetComponent<StatusFallingObject>());
            //ホールド対象から外し、次のオブジェクト生成のタイマーを初期化する
            holdingObjectCollider.enabled = true;
            holdingObjectCollider = null;
            holdingFallingObject = null;
            holdingObjectRigitbody2D.isKinematic = false;
            holdingObjectRigitbody2D = null;
            dropIntervalTimer = dropIntervalTime;
        }
    }

    private GameObject GenerateFallingObject()
    {
        //プレハブよりオブジェクトを複製
        var instantiateObject = Instantiate(fallingObjectPrefab, fallingObjectParentTransform.position, Quaternion.identity, fallingObjectParentTransform);
        //リストへ参照を保管
        fallingObjectList.Add(instantiateObject);
        var statusInstantiateObject = instantiateObject.GetComponent<StatusFallingObject>();
        statusFallingObjectList.Add(instantiateObject.GetComponent<StatusFallingObject>());
        fallingObjectSpriteRendererList.Add(instantiateObject.GetComponent<SpriteRenderer>());
        //StatusFallingObjectに書き込み
        statusInstantiateObject.NumberFallingObject = statusFallingObjectList.Count - 1;
        //タグでオブジェクトの状態を管理
        instantiateObject.tag = tagOfFallingObjectBeforeDrop;
        //タイプを乱数で決定、結果を反映
        int fallingObjectType = Random.Range(0, numberOfObjectType - 3);
        ChangeFallingObjectType(statusInstantiateObject, fallingObjectType);
        return instantiateObject;
    }

    //生成後から一定時間のみGameover判定を無視されるタグとする、awaitしないで処理を走らせておく
    //_ = TemporarilyIgnoreGameover(instantiateObject);
    private async UniTask TemporarilyIgnoreGameover(GameObject beChangedObject)
    {
        beChangedObject.tag = tagOfFallingObjectIgnoreGameover;
        await UniTask.Delay((int)ignoreGameoverJudgmentTime);
        beChangedObject.tag = tagOfFallingObject;
    }

    private void ChangeFallingObjectType(StatusFallingObject changedFallingObjectStatus, int newTypeValue)
    {
        GameObject ChangedObject = changedFallingObjectStatus.gameObject;
        //ステータスを更新
        changedFallingObjectStatus.TypeFallingObject = newTypeValue;
        //タイプに従ったスプライト、大きさに変更
        fallingObjectSpriteRendererList[changedFallingObjectStatus.NumberFallingObject].sprite = spriteList[newTypeValue];
        ChangedObject.transform.localScale = spriteScaleList[newTypeValue];
        //コライダーを更新（重い処理なのでオブジェクトの数が多いと厳しいかも）
        //コンポーネントを付けなおさないとコライダーがスプライトに沿った形に自動調整されない
        Destroy(ChangedObject.GetComponent<PolygonCollider2D>());
        ChangedObject.AddComponent<PolygonCollider2D>();
    }

    public void EventCollideFalilingObjects(StatusFallingObject collideObjectStatus, StatusFallingObject beCollidedObjectStatus)
    {
        //同じタイプのオブジェクトが触れた時のみ進化（ex. type 2 + type 2 -> type 1）
        if (collideObjectStatus.TypeFallingObject != beCollidedObjectStatus.TypeFallingObject) return;
        //進化させる、消去するオブジェクトを決定（上側にいるオブジェクトの方を進化させる）
        bool collideObjectIsChanged = false;
        Vector3 collideObjectPosition = collideObjectStatus.gameObject.transform.position;
        Vector3 beCollidedObjectPosition = beCollidedObjectStatus.gameObject.transform.position;
        bool isYPositionSame = collideObjectPosition.y == beCollidedObjectPosition.y;
        if (isYPositionSame)
        {
            bool isCollideObjectXPositionHigher = collideObjectPosition.x > beCollidedObjectPosition.x;
            collideObjectIsChanged = isCollideObjectXPositionHigher ? true : false;
        }
        else
        {
            bool isCollideObjectYPositionHigher = collideObjectPosition.y > beCollidedObjectPosition.y;
            collideObjectIsChanged = isCollideObjectYPositionHigher ? true : false;
        }
        StatusFallingObject beChangedObjectStatus = collideObjectIsChanged ? collideObjectStatus : beCollidedObjectStatus;
        StatusFallingObject beNonActivedObjectStatus = collideObjectIsChanged ? beCollidedObjectStatus : collideObjectStatus;
        //進化
        int currentTypeValue = beChangedObjectStatus.TypeFallingObject;
        int newTypeValue = currentTypeValue + 1;
        if (newTypeValue == numberOfObjectType)
        {
            //スコア加算等を追加する予定、今は取り敢えず非アクティブ化（画面から消す）
            beChangedObjectStatus.gameObject.SetActive(false);
        }
        else
        {
            ChangeFallingObjectType(beChangedObjectStatus, newTypeValue);
        }
        //進化させない方のオブジェクトは非アクティブ化（画面から消す）
        beNonActivedObjectStatus.gameObject.SetActive(false);
    }

     public void EventCollideGameoverLine(StatusFallingObject collideObjectStatus)
    {
        if (collideObjectStatus.gameObject.CompareTag(tagOfFallingObjectIgnoreGameover)) return;
        mainSceneController.NowMainSceneSituation = MainSceneSituation.Gameover;
        Debug.Log(collideObjectStatus.name + "Gameover");
    }
}
