using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class TitleSceneController : MonoBehaviour
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private Button startButton;

    void Start()
    {
        string errorMassage = "インスペクター未記入";
        if (startButton == null) Debug.LogError(errorMassage);
        startButton.onClick.AddListener(() => SceneManager.LoadScene(SceneName.MainScene.ToString()));
    }
}
