using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOnTriggerEnter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay");
    }
}
