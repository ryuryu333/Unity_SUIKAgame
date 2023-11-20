using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static MainSceneController;

public class EventIndividualFallingObjects : MonoBehaviour
{
    private StatusFallingObject statusFallingObject;
    private string tagOfFallingObject;
    private string tagOfGameoverLine;
    private string tagOfFallingObjectIgnorGameover;
    private MainSceneController mainSceneController;

    private void Start()
    {
        mainSceneController = MainSceneController.Instance;
        tagOfFallingObject = GameObjectTag.FallingObject.ToString();
        tagOfFallingObjectIgnorGameover = GameObjectTag.FallingObjectIgnoreGameover.ToString();
        tagOfGameoverLine = GameObjectTag.GameoverLine.ToString();
        statusFallingObject = gameObject.GetComponent<StatusFallingObject>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mainSceneController.NowMainSceneSituation != MainSceneSituation.Playing) return;
        bool isFallingObject = collision.gameObject.CompareTag(tagOfFallingObject) || collision.gameObject.CompareTag(tagOfFallingObjectIgnorGameover);
        if (isFallingObject)
        {
            StatusFallingObject collidedObjectStatus = collision.gameObject.GetComponent<StatusFallingObject>();
            FallingObjectController.Instance.EventCollideFalilingObjects(statusFallingObject, collidedObjectStatus);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (mainSceneController.NowMainSceneSituation != MainSceneSituation.Playing) return;
        if (collision.gameObject.CompareTag(tagOfGameoverLine))
        {
            FallingObjectController.Instance.EventCollideGameoverLine(statusFallingObject);
        }
    }
}
