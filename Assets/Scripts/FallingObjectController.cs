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
    //[SerializeField] private List<Sprite> objcetSpriteList = new List<Sprite>(); ��Ŏ���


    private void FallingObjectGenerate()
    {
        Vector3 generatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        generatePosition.z = 0; //z���W�̎擾�l�̓J�����Ɠ����A�J�����ɉf��悤��z���W��ύX
        var instantiateObject = Instantiate(fallingObjectPrefab, generatePosition, Quaternion.identity, fallingObjectParentTransform);
        fallingObjectList.Add(instantiateObject);
        //��ނ̌���
        int fallingObjectType = Random.Range(0, numberOfObjectType);
        //�傫����ύX�@ex. Type 1�F1�{�AType2�F0.7�{
        float modifiedScale = instantiateObject.transform.localScale.x * objcetScaleByTypeList[fallingObjectType];
        instantiateObject.transform.localScale = new Vector3(modifiedScale, modifiedScale, modifiedScale);
        //�I�u�W�F�N�g�ɕ\������鐔����ύX�A�����I�ɂ̓X�v���C�g�������ւ��鏈���ɕύX����
        TextMeshProUGUI text = instantiateObject.transform.Find("Canvas/Text").GetComponent<TextMeshProUGUI>();
        text.text = (fallingObjectType + 1).ToString();
        Debug.Log(text);
    }

}
