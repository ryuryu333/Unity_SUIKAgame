using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusFallingObject : MonoBehaviour
{
    [SerializeField] private int numberFallingObject;
    public int NumberFallingObject { get => numberFallingObject; set => numberFallingObject = value; }
    [SerializeField] private int typeFallingObject;
    public int TypeFallingObject { get => typeFallingObject; set => typeFallingObject = value; }

}