using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private GameObject uiResultObject;
    [SerializeField] private Button changeToTitleScene;

    public void Initialization()
    {
        string errorMassage = "インスペクター未記入";
        if (uiResultObject == null || changeToTitleScene == null) Debug.LogError(errorMassage);
        uiResultObject.SetActive(false);
        changeToTitleScene.onClick.AddListener(() => SceneManager.LoadScene(SceneName.TitleScene.ToString()));
    }

    public void DisplayUIResult()
    {
        uiResultObject.SetActive(true);
    }
}
