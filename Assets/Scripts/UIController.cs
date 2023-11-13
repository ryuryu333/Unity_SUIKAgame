using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [Header("インスペクタで値を指定")]
    [SerializeField] private GameObject UIResultObject;


    public void Initialization()
    {
        string errorMassage = "インスペクター未記入";
        if (UIResultObject == null) Debug.LogError(errorMassage);
        UIResultObject.SetActive(false);
    }

    public void DisplayUIResult()
    {
        UIResultObject.SetActive(true);
    }
}
