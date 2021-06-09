using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sync camera's position with Y position of the target object. It has some prohibitions.
public class CameraFollow : MonoBehaviour
{
    [Tooltip("The target object to follow.")]
    public Transform target;

    [Tooltip("The highest point where camera may locate.")]
    public float topLimit = 10.0f;

    [Tooltip("The lowest point where camera may locate.")]
    public float bottomLimit = -10.0f;

    [Tooltip("The following speed.")]
    public float followSpeed = 0.5f;

    // Set camera position after all object positions updating.
    void LateUpdate()
    {
        // If target object exists...
        if (target != null)
        {
            // Get its position
            var newPosition = transform.position;
            
            // Work out where this camera should be
            newPosition.y = Mathf.Lerp(newPosition.y, target.position.y, followSpeed);
            
            // Clamp this new location to within our limits
            newPosition.y = Mathf.Min(newPosition.y, topLimit);
            newPosition.y = Mathf.Max(newPosition.y, bottomLimit);
            
            // Update our location
            transform.position = newPosition;
        }
    }

    // When selected in the editor, draw a line from the top limit to the 
    // bottom.
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        var position = transform.position;
        var topPoint = new Vector3(position.x, topLimit, position.z);
        var bottomPoint = new Vector3(position.x, bottomLimit, position.z);
        
        Gizmos.DrawLine(topPoint, bottomPoint);
    }
}
