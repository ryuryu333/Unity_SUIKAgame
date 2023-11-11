using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventIndividualFallingObjects : MonoBehaviour
{
    private StatusFallingObject statusFallingObject;
    private string tagOfFallingObject;
    private string tagOfGameoverLine;

    private void Start()
    {
        tagOfFallingObject = MainSceneController.Instance.TagOfFallingObject;
        tagOfGameoverLine = MainSceneController.Instance.TagOfGameoverLine;
        statusFallingObject = gameObject.GetComponent<StatusFallingObject>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagOfFallingObject))
        {
            StatusFallingObject collidedObjectStatus = collision.gameObject.GetComponent<StatusFallingObject>();
            FallingObjectController.Instance.EventCollideFalilingObjects(statusFallingObject, collidedObjectStatus);
        }
        else if (collision.gameObject.CompareTag(tagOfGameoverLine))
        {
            //FallingObjectController.Instance.EventCollideGameoverLine(statusFallingObject);
        }
    }
}
