using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;

public class FallingObjectController : SingletonMonoBehaviour<FallingObjectController>
{
    [SerializeField] private GameObject fallingObjectPrefab;
    [SerializeField] private GameObject fallingObjectParent;
    private Transform fallingObjectParentTransform;
    private List<GameObject> fallingObjectList = new();
    private List<StatusFallingObject> statusFallingObjectList = new();
    [SerializeField] private float generationIntervalValue;
    [SerializeField] private float generationIntervalTimer;
    [SerializeField] private List<float> objcetScaleByTypeList = new List<float>();
    private int numberOfObjectType;

    [SerializeField] private List<AssetReference> SpriteRefList = new();
    [SerializeField] private List<Sprite> SpriteList = new();

    public float GenerationIntervalValue
    { get { return generationIntervalValue; } set { generationIntervalValue = value; } }

    // Start is called before the first frame update
    void Start()
    {
        numberOfObjectType = objcetScaleByTypeList.Count;
        fallingObjectParentTransform = fallingObjectParent.transform;
        generationIntervalTimer = generationIntervalValue;
    }

    // Update is called once per frame
    void Update()
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
            generationIntervalTimer = generationIntervalValue;
            FallingObjectGenerate();
        }
    }



    private void Initialize()
    {
        numberOfObjectType = objcetScaleByTypeList.Count;
        fallingObjectParentTransform = fallingObjectParent.transform;
        generationIntervalTimer = generationIntervalValue;
    }


    private void FallingObjectGenerate()
    {
        //プレハブよりオブジェクトを複製
        Vector3 generatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        generatePosition.z = 0; //z座標の取得値はカメラと同じ、カメラに映るようにz座標を変更
        var instantiateObject = Instantiate(fallingObjectPrefab, generatePosition, Quaternion.identity, fallingObjectParentTransform);
        //リストへ参照を保管
        fallingObjectList.Add(instantiateObject);
        StatusFallingObject statusInstantiateObject = instantiateObject.GetComponent<StatusFallingObject>();
        statusFallingObjectList.Add(statusInstantiateObject);
        //何個目に作成されたオブジェクトか書き込み、List[NumberFallingObject]番目で保管したリストへアクセス
        statusInstantiateObject.NumberFallingObject = statusFallingObjectList.Count - 1;
        //タイプの決定、反映
        int fallingObjectType = Random.Range(1, numberOfObjectType + 1);
        ChangeFallingObjectStatus(statusInstantiateObject, fallingObjectType);
    }

    private void ChangeFallingObjectStatus(StatusFallingObject changedFallingObjectStatus, int newTypeValue)
    {
        GameObject ChangedObject = changedFallingObjectStatus.gameObject;
        //ステータスを変更
        changedFallingObjectStatus.TypeFallingObject = newTypeValue;
        //大きさを変更　ex. Type 1：1倍、Type2：0.7倍
        float modifiedScale = objcetScaleByTypeList[newTypeValue - 1];
        ChangedObject.transform.localScale = new Vector3(modifiedScale, modifiedScale, modifiedScale);
        //オブジェクトに表示される数字を変更、将来的にはスプライトを差し替える処理に変更する
        TextMeshProUGUI text = ChangedObject.transform.Find("Canvas/Text").GetComponent<TextMeshProUGUI>();
        string objectName = (newTypeValue).ToString();
        text.text = objectName;
    }

    public void EventCollideFalilingObjects(StatusFallingObject collideObjectStatus, StatusFallingObject beCollidedObjectStatus)
    {
        //同じタイプのオブジェクトが触れた時のみ進化（ex. type 2 + type 2 -> type 1）
        if (collideObjectStatus.TypeFallingObject != beCollidedObjectStatus.TypeFallingObject) return;
        //進化するオブジェクトを決定（もう片方のオブジェクトは非アクティブ化）
        (StatusFallingObject beChangedObjectStatus, StatusFallingObject inactiveObjectStatus) collideObjectsstatuses = (null, null);
        Vector3 collideObjectPosition = collideObjectStatus.gameObject.transform.position;
        Vector3 beCollidedObjectPosition = beCollidedObjectStatus.gameObject.transform.position;
        if (collideObjectPosition.y == beCollidedObjectPosition.y)
        {
            if (collideObjectPosition.x > beCollidedObjectPosition.x) collideObjectsstatuses = (collideObjectStatus, beCollidedObjectStatus);
            else collideObjectsstatuses = (beCollidedObjectStatus, collideObjectStatus);
        }
        else
        {
            if (collideObjectPosition.y > beCollidedObjectPosition.y) collideObjectsstatuses = (collideObjectStatus, beCollidedObjectStatus);
            else collideObjectsstatuses = (beCollidedObjectStatus, collideObjectStatus);
        }
        //進化
        int currentTypeValue = collideObjectsstatuses.beChangedObjectStatus.TypeFallingObject;
        if (currentTypeValue == 1)
        {
            //スコア加算等を追加する予定
            collideObjectsstatuses.beChangedObjectStatus.gameObject.SetActive(false);
        }
        else
        {
            ChangeFallingObjectStatus(collideObjectsstatuses.beChangedObjectStatus, currentTypeValue - 1);
        }
        //進化元でないオブジェクトは非アクティブ化
        collideObjectsstatuses.inactiveObjectStatus.gameObject.SetActive(false);
    }
}
