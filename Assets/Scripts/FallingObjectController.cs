using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class FallingObjectController : MonoBehaviour
{
    [SerializeField] private GameObject fallingObjectPrefab;
    [SerializeField] private GameObject fallingObjectParent; 
    private Transform fallingObjectParentTransform;
    private List<GameObject> fallingObjectList = new List<GameObject>();
    [SerializeField] private float generationIntervalValue;
    [SerializeField] private float generationIntervalTimer;

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

    [SerializeField] private List<float> objcetScaleByTypeList = new List<float>();
    private int numberOfObjectType;
    //[SerializeField] private List<Sprite> objcetSpriteList = new List<Sprite>(); 後で実装


    private void FallingObjectGenerate()
    {
        Vector3 generatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        generatePosition.z = 0; //z座標の取得値はカメラと同じ、カメラに映るようにz座標を変更
        var instantiateObject = Instantiate(fallingObjectPrefab, generatePosition, Quaternion.identity, fallingObjectParentTransform);
        fallingObjectList.Add(instantiateObject);
        //種類の決定
        int fallingObjectType = Random.Range(0, numberOfObjectType);
        //大きさを変更　ex. Type 1：1倍、Type2：0.7倍
        float modifiedScale = instantiateObject.transform.localScale.x * objcetScaleByTypeList[fallingObjectType];
        instantiateObject.transform.localScale = new Vector3(modifiedScale, modifiedScale, modifiedScale);
        //オブジェクトに表示される数字を変更、将来的にはスプライトを差し替える処理に変更する
        TextMeshProUGUI text = instantiateObject.transform.Find("Canvas/Text").GetComponent<TextMeshProUGUI>();
        text.text = (fallingObjectType + 1).ToString();
        Debug.Log(text);
    }

}
