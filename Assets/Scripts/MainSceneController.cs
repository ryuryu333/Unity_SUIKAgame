using UnityEngine;
using MainSceneEnumList;

public class MainSceneController : SingletonMonoBehaviour<MainSceneController>
{
    [Header("デバック用")]
    private FallingObjectController fallingObjectController;
    private UIController uiController;
    private ScoreController scoreController;
    [SerializeField] private int score = 0;
    public int Score
    {
        get => score;
        set
        {
            score = value;
            uiController.UIScoreUpdate();
        }
    }
    [SerializeField] private MainSceneSituation nowMainSceneSituation;
    public MainSceneSituation NowMainSceneSituation 
    { 
        get => nowMainSceneSituation; 
        set      
        {
            if (value == MainSceneSituation.Gameover) GameoverEvent();
            nowMainSceneSituation = value;
        }
    }

    async void Start()
    {
        nowMainSceneSituation = MainSceneSituation.Initializing;
        fallingObjectController = FallingObjectController.Instance;
        uiController = UIController.Instance;
        scoreController = ScoreController.Instance;
        await fallingObjectController.Initialization();
        uiController.Initialization();
        scoreController.Initialization();
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

    private void GameoverEvent()
    {
        uiController.GameoverEvent();
    }
}
