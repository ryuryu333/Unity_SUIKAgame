using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FallingObjectController : SingletonMonoBehaviour<FallingObjectController>
{
    [Header("�C���X�y�N�^�Œl���w��")]
    [SerializeField] private GameObject fallingObjectPrefab;
    [SerializeField] private GameObject fallingObjectParent;
    [SerializeField] private List<float> objcetScaleByTypeList = new();
    [SerializeField] private List<AssetReferenceSprite> spriteRefList = new();
    [SerializeField] private float generationIntervalTime;
    [SerializeField] private float ignoreGameoverJudgmentTime;

    //�l�̃L���b�V���p�ϐ�
    [Header("�f�o�b�N�p")]
    [SerializeField] private List<Sprite> spriteList = new();
    private List<Vector3> spriteScaleList = new();
    private bool endInitialization = false;
    private Transform fallingObjectParentTransform;
    private List<GameObject> fallingObjectList = new();
    private List<StatusFallingObject> statusFallingObjectList = new();
    private List<SpriteRenderer> fallingObjectSpriteRendererList = new();
    private int numberOfObjectType;

    [SerializeField] private float generationIntervalTimer;
    public float GenerationIntervalValue { get => generationIntervalTime; set => generationIntervalTime = value; }

    async void Start()
    {
        string errorMassage = "�C���X�y�N�^�[���L��";
        if (fallingObjectPrefab == null || fallingObjectParent == null || objcetScaleByTypeList.Count == 0 || spriteRefList.Count == 0 || generationIntervalTime == 0) Debug.LogError(errorMassage);
        //�ݒ肵���X�v���C�g�����ɓǂݍ���
        for (int i = 0; i < spriteRefList.Count; i++)
        {
            //FallingObject�p�̃X�v���C�g��񓯊��Ń��[�h
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(spriteRefList[i]);
            //���[�h�������ɌĂяo�����C�x���g���w��
            handle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Failed) Debug.LogError("AssetReference failed to load.FallingObjcetSprite");
                spriteList.Add(handle.Result);
            };
            await handle.Task;
            //�X�v���C�g�̑傫�����擾���AFallingObject�ɃX�v���C�g���A�^�b�`�����ۂɎw�肵���傫���ɂł���g��{�����v�Z
            float scaleX = objcetScaleByTypeList[i] / spriteList[i].bounds.size.x;
            float scaleY = objcetScaleByTypeList[i] / spriteList[i].bounds.size.y;
            spriteScaleList.Add(new Vector3(scaleX, scaleY, 1.0f));
        }
        //�����o�ϐ��̏�����
        numberOfObjectType = objcetScaleByTypeList.Count;
        fallingObjectParentTransform = fallingObjectParent.transform;
        generationIntervalTimer = generationIntervalTime;
        //Tmp�I�u�W�F�N�g���폜
        Destroy(GameObject.Find("Tmp"));        
        endInitialization = true;
    }

    void Update()
    {
        if (!endInitialization) return;
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
        //�v���n�u���I�u�W�F�N�g�𕡐�
        Vector3 generatePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        generatePosition.z = 0; //z���W�̎擾�l�̓J�����Ɠ����A�J�����ɉf��悤��z���W��ύX
        var instantiateObject = Instantiate(fallingObjectPrefab, generatePosition, Quaternion.Euler(0, 0, Random.Range(0, 360)), fallingObjectParentTransform);
        //���X�g�֎Q�Ƃ�ۊ�
        fallingObjectList.Add(instantiateObject);
        StatusFallingObject statusInstantiateObject = instantiateObject.GetComponent<StatusFallingObject>();
        statusFallingObjectList.Add(instantiateObject.GetComponent<StatusFallingObject>());
        fallingObjectSpriteRendererList.Add(instantiateObject.GetComponent<SpriteRenderer>());
        //StatusFallingObject�ɏ�������
        statusInstantiateObject.NumberFallingObject = statusFallingObjectList.Count - 1;
        statusInstantiateObject.IsignoreGameoverJudgment = true;
        //�����ォ���莞�Ԍ��false�ɏ������������̂ŁAawait���Ȃ��ŏ����𑖂点�Ă���
        _ = ChangeFlagAfterIgnoreGameoverJudgmentTimeAsync(statusInstantiateObject);
        //�^�C�v�𗐐��Ō���A���ʂ𔽉f
        int fallingObjectType = Random.Range(0, numberOfObjectType - 3);
        ChangeFallingObjectType(statusInstantiateObject, fallingObjectType);
    }

    private async UniTask ChangeFlagAfterIgnoreGameoverJudgmentTimeAsync(StatusFallingObject statusInstantiateObject)
    {
        await UniTask.Delay((int)ignoreGameoverJudgmentTime);
        statusInstantiateObject.IsignoreGameoverJudgment = false;
    }

    private void ChangeFallingObjectType(StatusFallingObject changedFallingObjectStatus, int newTypeValue)
    {
        GameObject ChangedObject = changedFallingObjectStatus.gameObject;
        //�X�e�[�^�X���X�V
        changedFallingObjectStatus.TypeFallingObject = newTypeValue;
        //�^�C�v�ɏ]�����X�v���C�g�A�傫���ɕύX
        fallingObjectSpriteRendererList[changedFallingObjectStatus.NumberFallingObject].sprite = spriteList[newTypeValue];
        ChangedObject.transform.localScale = spriteScaleList[newTypeValue];
        //�R���C�_�[���X�V�i�d�������Ȃ̂ŃI�u�W�F�N�g�̐��������ƌ����������j
        //�R���|�[�l���g��t���Ȃ����Ȃ��ƃR���C�_�[���X�v���C�g�ɉ������`�Ɏ�����������Ȃ�
        Destroy(ChangedObject.GetComponent<PolygonCollider2D>());
        ChangedObject.AddComponent<PolygonCollider2D>();
    }

    public void EventCollideFalilingObjects(StatusFallingObject collideObjectStatus, StatusFallingObject beCollidedObjectStatus)
    {
        //�����^�C�v�̃I�u�W�F�N�g���G�ꂽ���̂ݐi���iex. type 2 + type 2 -> type 1�j
        if (collideObjectStatus.TypeFallingObject != beCollidedObjectStatus.TypeFallingObject) return;
        //�i��������A��������I�u�W�F�N�g������i�㑤�ɂ���I�u�W�F�N�g�̕���i��������j
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
        //�i��
        int currentTypeValue = beChangedObjectStatus.TypeFallingObject;
        int newTypeValue = currentTypeValue + 1;
        if (newTypeValue == numberOfObjectType)
        {
            //�X�R�A���Z����ǉ�����\��A���͎�芸������A�N�e�B�u���i��ʂ�������j
            beChangedObjectStatus.gameObject.SetActive(false);
        }
        else
        {
            ChangeFallingObjectType(beChangedObjectStatus, newTypeValue);
        }
        //�i�������Ȃ����̃I�u�W�F�N�g�͔�A�N�e�B�u���i��ʂ�������j
        beNonActivedObjectStatus.gameObject.SetActive(false);
    }

     
}
