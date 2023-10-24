using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventIndividualFallingObjects : MonoBehaviour
{
    private string tagOfFallingObject;
    private StatusFallingObject statusFallingObject;

    private void Start()
    {
        tagOfFallingObject = gameObject.tag;
        statusFallingObject = gameObject.GetComponent<StatusFallingObject>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagOfFallingObject))
        {
            StatusFallingObject collidedObjectStatus = collision.gameObject.GetComponent<StatusFallingObject>();
            FallingObjectController.Instance.EventCollideFalilingObjects(statusFallingObject, collidedObjectStatus);
        }
    }
}
