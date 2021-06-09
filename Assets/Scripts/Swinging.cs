using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uses input manager to enforce side motion to object.
public class Swinging : MonoBehaviour
{
    [Tooltip("The range of displacement. Bigger number -> higher displacement.")]
    public float swingSensitivity = 100.0f;

    // Use this method to make working with physical engine easier.
    private void FixedUpdate()
    {
        var rigidBody = GetComponent<Rigidbody2D>();
        
        // If no rigid body -> remove this component.
        if (rigidBody == null)
        {
            Destroy(this);
            return;
        }
        
        // Get side motion measure from input manager.
        var swing = InputManager.instance.sidewaysMotion;
        
        // Calculate the used force.
        var force = new Vector2(swing * swingSensitivity, 0);
        
        // Use the force.
        rigidBody.AddForce(force);
    }
}
