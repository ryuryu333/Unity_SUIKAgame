using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : SingletonMonoBehaviour<ScoreController>
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private List<int> scoreByTypeList = new();
    MainSceneController mainSceneController;

    public void Initialization()
    {
        string errorMassage = "インスペクター未記入";
        if (scoreByTypeList.Count == 0) Debug.LogError(errorMassage);
        mainSceneController = MainSceneController.Instance;
    }

    public void ScoreWhenDropFallingObject(StatusFallingObject generatedObjectStatus)
    {
        int addScore = scoreByTypeList[generatedObjectStatus.TypeFallingObject];
        mainSceneController.Score += addScore;
    }

}
