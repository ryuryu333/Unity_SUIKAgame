using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [Header("�C���X�y�N�^�Œl���w��")]
    [SerializeField] private GameObject UIResultObject;


    public void Initialization()
    {
        string errorMassage = "�C���X�y�N�^�[���L��";
        if (UIResultObject == null) Debug.LogError(errorMassage);
        UIResultObject.SetActive(false);
    }

    public void DisplayUIResult()
    {
        UIResultObject.SetActive(true);
    }
}
