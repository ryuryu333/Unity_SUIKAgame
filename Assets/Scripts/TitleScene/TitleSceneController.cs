using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class TitleSceneController : MonoBehaviour
{
    [Header("�C���X�y�N�^�Œl���w��")]
    [SerializeField] private Button startButton;

    void Start()
    {
        string errorMassage = "�C���X�y�N�^�[���L��";
        if (startButton == null) Debug.LogError(errorMassage);
        startButton.onClick.AddListener(() => SceneManager.LoadScene(SceneName.MainScene.ToString()));
    }
}
