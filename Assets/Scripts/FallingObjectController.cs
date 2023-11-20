using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static MainSceneController;
using static UnityEngine.GraphicsBuffer;

public class FallingObjectController : SingletonMonoBehaviour<FallingObjectController>
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private GameObject fallingObjectPrefab;
    [SerializeField] private GameObject fallingObjectParent;
    [SerializeField] private List<float> objcetScaleByTypeList = new();
    [SerializeField] private List<AssetReferenceSprite> spriteRefList = new();
    [SerializeField] private float generationIntervalTime;
    [SerializeField] private float ignoreGameoverJudgmentTime;

    //値のキャッシュ用変数
    [Header("デバック用")]
    [SerializeField] private List<Sprite> spriteList = new();
    private List<Vector3> spriteScaleList = new();
    private Transform fallingObjectParentTransform;
    private List<GameObject> fallingObjectList = new();
    private List<StatusFallingObject> statusFallingObjectList = new();
    private List<SpriteRenderer> fallingObjectSpriteRendererList = new();
    private int numberOfObjectType;
    private string tagOfFallingObject;
    private string tagOfFallingObjectIgnoreGameover;
    private MainSceneController mainSceneController;
    private ScoreController scoreController;

    [SerializeField] private float generationIntervalTimer;
    public float GenerationIntervalValue { get => generationIntervalTime; set => generationIntervalTime = value; }

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
        mainSceneController = MainSceneController.Instance;
        scoreController = ScoreController.Instance;
        tagOfFallingObjectIgnoreGameover = mainSceneController.TagOfFallingObjectIgnoreGameover;
        tagOfFallingObject = mainSceneController.TagOfFallingObject;
        //Tmpオブジェクトを削除
        Destroy(GameObject.Find("Tmp"));
    }

    public void UpdateMe()
    {
        CheckPlayerInput();
    }

    private void CheckPlayerInput()
    {
        if (generationIntervalTimer > 0)
        {
            generationIntervalTimer--;
            return;
        }
        if (Input.GetMouseButtonUp(0))
        {
            generationIntervalTimer = generationIntervalTime;
            FallingObjectGenerate();
        }
    }


    private void FallingObjectGenerate()
    {
        //プレハブよりオブジェクトを複製
        Vector3 generatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        generatePosition.z = 0; //z座標の取得値はカメラと同じ、カメラに映るようにz座標を変更
        var instantiateObject = Instantiate(fallingObjectPrefab, generatePosition, Quaternion.Euler(0, 0, Random.Range(0, 360)), fallingObjectParentTransform);
        //リストへ参照を保管
        fallingObjectList.Add(instantiateObject);
        StatusFallingObject statusInstantiateObject = instantiateObject.GetComponent<StatusFallingObject>();
        statusFallingObjectList.Add(instantiateObject.GetComponent<StatusFallingObject>());
        fallingObjectSpriteRendererList.Add(instantiateObject.GetComponent<SpriteRenderer>());
        //StatusFallingObjectに書き込み
        statusInstantiateObject.NumberFallingObject = statusFallingObjectList.Count - 1;
        statusInstantiateObject.IsignoreGameoverJudgment = true;
        //生成後から一定時間はGameover判定を無視、awaitしないで処理を走らせておく
        _ = TemporarilyIgnoreGameover(instantiateObject);
        //タイプを乱数で決定、結果を反映
        int fallingObjectType = Random.Range(0, numberOfObjectType - 3);
        ChangeFallingObjectType(statusInstantiateObject, fallingObjectType);
    }

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
        //オブジェクト作成時にタイプに応じたスコアを加算
        scoreController.ScoreWhenGenerateFallingObject(changedFallingObjectStatus);
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
