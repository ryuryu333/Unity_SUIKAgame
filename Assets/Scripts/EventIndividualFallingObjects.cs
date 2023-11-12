using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MainSceneController;

public class EventIndividualFallingObjects : MonoBehaviour
{
    private StatusFallingObject statusFallingObject;
    private string tagOfFallingObject;
    private string tagOfGameoverLine;
    private MainSceneController mainSceneController;

    private void Start()
    {
        mainSceneController = MainSceneController.Instance;
        tagOfFallingObject = mainSceneController.TagOfFallingObject;
        tagOfGameoverLine = mainSceneController.TagOfGameoverLine;
        statusFallingObject = gameObject.GetComponent<StatusFallingObject>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mainSceneController.NowMainSceneSituation != MainSceneSituation.Playing) return;
        if (collision.gameObject.CompareTag(tagOfFallingObject))
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
