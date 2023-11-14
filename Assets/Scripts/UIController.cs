using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [Header("�C���X�y�N�^�Œl���w��")]
    [SerializeField] private GameObject uiResultObject;
    [SerializeField] private Button changeToTitleScene;

    public void Initialization()
    {
        string errorMassage = "�C���X�y�N�^�[���L��";
        if (uiResultObject == null || changeToTitleScene == null) Debug.LogError(errorMassage);
        uiResultObject.SetActive(false);
        changeToTitleScene.onClick.AddListener(() => SceneManager.LoadScene(SceneName.TitleScene.ToString()));
    }

    public void DisplayUIResult()
    {
        uiResultObject.SetActive(true);
    }
}
