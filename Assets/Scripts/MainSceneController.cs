using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : SingletonMonoBehaviour<MainSceneController>
{
    [SerializeField] private string tagOfFallingObject;
    public string TagOfFallingObject { get => tagOfFallingObject; set => tagOfFallingObject = value; }

    [SerializeField] private string tagOfGameoverLine;
    public string TagOfGameoverLine { get => tagOfGameoverLine; set => tagOfGameoverLine = value; }

    // Start is called before the first frame update
    void Start()
    {
        string errorMassage = "インスペクター未記入";
        if (tagOfFallingObject == "" || tagOfGameoverLine == "") Debug.LogError(errorMassage);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
