using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Converts accelerometer data to the side displacement information.
public class InputManager : Singleton<InputManager>
{
    // Displacement measure. -1.0 = max left, 1.0 = max right.

    // Read only field.
    public float sidewaysMotion { get; private set; }

    // Update is called once per frame
    void Update()
    {
        var accel = Input.acceleration;
        sidewaysMotion = accel.x;
    }
}
