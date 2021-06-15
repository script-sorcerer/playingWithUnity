using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Destroy an object after a delay.
public class RemoveAfterDelay : MonoBehaviour
{
    [Tooltip("The delay before removing an object.")]
    public float delay = 1.0f;
    // Start is called before the first frame update.
    void Start()
    {
        // Kick off the 'Remove' coroutine.
        StartCoroutine(nameof(Remove));
    }

    IEnumerator Remove()
    {
        // Wait 'delay' seconds, and then destroy the gameObject attached to this object.
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
        
        // Don't say Destroy(this) - that just destroys this 
        // RemoveAfterDelay script.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
