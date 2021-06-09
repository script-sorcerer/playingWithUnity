using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The class helps to build singleton pattern flow.

// In order to use it - extend it. Example:
// public class MyManager : Singleton<MyManager> { }

// It grants ability to work with exactly one instance of passed class. Example:
// MyManager.instance.DoSomething();
public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    // The only one instance of the class.
    private static T _instance;

    // Access method. At the first time it creates a new instance,
    // after that it always returns the instance.
    public static T instance
    {
        get
        {
            // If no instance created.
            if (_instance == null)
            {
                // Tries to find object.
                _instance = FindObjectOfType<T>();
                
                // Print message on failure.
                if (_instance == null)
                {
                    Debug.LogErrorFormat("Can't find {0} !", typeof(T));
                }
            }

            // Return usable instance. 
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
