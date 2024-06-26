﻿using System;
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
    private TextMeshProUGUI scoreValueText;

    MainSceneController mainSceneController;

    public void Initialization()
    {
        string errorMassage = "インスペクター未記入";
        if (uiResultObject == null || changeToTitleScene == null || uiScore == null) Debug.LogError(errorMassage);
        mainSceneController = MainSceneController.Instance;
        uiResultObject.SetActive(false);
        uiScore.SetActive(true);
        scoreValueText = uiScore.transform.Find("Panel/ScoreValue").GetComponent<TextMeshProUGUI>();
        changeToTitleScene.onClick.AddListener(() => SceneManager.LoadScene(SceneName.TitleScene.ToString()));
    }

    public void GameoverEvent()
    {
        uiResultObject.SetActive(true);
    }

    public void UIScoreUpdate()
    {
        int currentScore = mainSceneController.Score;
        scoreValueText.text = currentScore.ToString();
    }
}
