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
        //�v���n�u���I�u�W�F�N�g�𕡐�
        Vector3 generatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        generatePosition.z = 0; //z���W�̎擾�l�̓J�����Ɠ����A�J�����ɉf��悤��z���W��ύX
        var instantiateObject = Instantiate(fallingObjectPrefab, generatePosition, Quaternion.identity, fallingObjectParentTransform);
        //���X�g�֎Q�Ƃ�ۊ�
        fallingObjectList.Add(instantiateObject);
        StatusFallingObject statusInstantiateObject = instantiateObject.GetComponent<StatusFallingObject>();
        statusFallingObjectList.Add(statusInstantiateObject);
        //���ڂɍ쐬���ꂽ�I�u�W�F�N�g���������݁AList[NumberFallingObject]�Ԗڂŕۊǂ������X�g�փA�N�Z�X
        statusInstantiateObject.NumberFallingObject = statusFallingObjectList.Count - 1;
        //�^�C�v�̌���A���f
        int fallingObjectType = Random.Range(1, numberOfObjectType + 1);
        ChangeFallingObjectStatus(statusInstantiateObject, fallingObjectType);
    }

    private void ChangeFallingObjectStatus(StatusFallingObject changedFallingObjectStatus, int newTypeValue)
    {
        GameObject ChangedObject = changedFallingObjectStatus.gameObject;
        //�X�e�[�^�X��ύX
        changedFallingObjectStatus.TypeFallingObject = newTypeValue;
        //�傫����ύX�@ex. Type 1�F1�{�AType2�F0.7�{
        float modifiedScale = objcetScaleByTypeList[newTypeValue - 1];
        ChangedObject.transform.localScale = new Vector3(modifiedScale, modifiedScale, modifiedScale);
        //�I�u�W�F�N�g�ɕ\������鐔����ύX�A�����I�ɂ̓X�v���C�g�������ւ��鏈���ɕύX����
        TextMeshProUGUI text = ChangedObject.transform.Find("Canvas/Text").GetComponent<TextMeshProUGUI>();
        string objectName = (newTypeValue).ToString();
        text.text = objectName;
    }

    public void EventCollideFalilingObjects(StatusFallingObject collideObjectStatus, StatusFallingObject beCollidedObjectStatus)
    {
        //�����^�C�v�̃I�u�W�F�N�g���G�ꂽ���̂ݐi���iex. type 2 + type 2 -> type 1�j
        if (collideObjectStatus.TypeFallingObject != beCollidedObjectStatus.TypeFallingObject) return;
        //�i������I�u�W�F�N�g������i�����Е��̃I�u�W�F�N�g�͔�A�N�e�B�u���j
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
        //�i��
        int currentTypeValue = collideObjectsstatuses.beChangedObjectStatus.TypeFallingObject;
        if (currentTypeValue == 1)
        {
            //�X�R�A���Z����ǉ�����\��
            collideObjectsstatuses.beChangedObjectStatus.gameObject.SetActive(false);
        }
        else
        {
            ChangeFallingObjectStatus(collideObjectsstatuses.beChangedObjectStatus, currentTypeValue - 1);
        }
        //�i�����łȂ��I�u�W�F�N�g�͔�A�N�e�B�u��
        collideObjectsstatuses.inactiveObjectStatus.gameObject.SetActive(false);
    }
}
