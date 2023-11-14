using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MainSceneController : SingletonMonoBehaviour<MainSceneController>
{
    [Header("�C���X�y�N�^�Œl���w��")]
    [SerializeField] private string tagOfFallingObject;
    public string TagOfFallingObject { get => tagOfFallingObject; set => tagOfFallingObject = value; }

    [SerializeField] private string tagOfGameoverLine;
    public string TagOfGameoverLine { get => tagOfGameoverLine; set => tagOfGameoverLine = value; }

    [SerializeField] private string tagOfFallingObjectIgnoreGameover;
    public string TagOfFallingObjectIgnoreGameover { get => tagOfFallingObjectIgnoreGameover; set => tagOfFallingObjectIgnoreGameover = value; }

    [Header("�f�o�b�N�p")]
    private FallingObjectController fallingObjectController;
    private UIController uiController;
    [SerializeField] private MainSceneSituation nowMainSceneSituation;
    public MainSceneSituation NowMainSceneSituation 
    { 
        get => nowMainSceneSituation; 
        set      
        {
            if (value == MainSceneSituation.Gameover) DisplayUIResult();
            nowMainSceneSituation = value;
        }         
    }


    public enum MainSceneSituation
    {
        Initializing,
        Playing,
        Pause,
        Gameover
    }

    async void Start()
    {
        nowMainSceneSituation = MainSceneSituation.Initializing;
        string errorMassage = "�C���X�y�N�^�[���L��";
        if (tagOfFallingObject == "" || tagOfGameoverLine == "" || tagOfFallingObjectIgnoreGameover == "") Debug.LogError(errorMassage);
        fallingObjectController = FallingObjectController.Instance;
        uiController = UIController.Instance;
        await fallingObjectController.Initialization();
        uiController.Initialization();
        nowMainSceneSituation = MainSceneSituation.Playing;
    }

    void Update()
    {
        switch (nowMainSceneSituation)
        {
            case MainSceneSituation.Initializing:
                break;
            case MainSceneSituation.Playing:
                fallingObjectController.UpdateMe();
                break;    
            case MainSceneSituation.Pause:
                break;
            case MainSceneSituation.Gameover:
                break;
            default:
                break;
        }
    }

    private void DisplayUIResult()
    {
        uiController.DisplayUIResult();
    }
}
