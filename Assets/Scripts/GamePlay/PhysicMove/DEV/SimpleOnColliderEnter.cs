using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOnColliderEnter : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collider Enter");
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        Debug.Log("Collider Stay");
    }
    
    private void OnCollisionExit(Collision collisionInfo)
    {
        Debug.Log("Collider Exit");
    }
}
