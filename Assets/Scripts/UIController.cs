using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private GameObject uiResultObject;
    [SerializeField] private Button changeToTitleScene;
    [SerializeField] private GameObject uiScore;
    private TextMeshPro scoreValueText;

    public void Initialization()
    {
        string errorMassage = "インスペクター未記入";
        if (uiResultObject == null || changeToTitleScene == null || uiScore == null) Debug.LogError(errorMassage);
        uiResultObject.SetActive(false);
        uiScore.SetActive(true);
        scoreValueText = uiScore.transform.Find("Panel/ScoreValue").GetComponent<TextMeshPro>();  
        changeToTitleScene.onClick.AddListener(() => SceneManager.LoadScene(SceneName.TitleScene.ToString()));
    }

    public void GameoverEvent()
    {
        uiResultObject.SetActive(true);
    }

    private void ScoreUpdate(int currentScore)
    {
        scoreValueText.text = currentScore.ToString();
    }
}
