using UnityEngine;
using UnityEngine.Events;

// Calls UnityEvent when object with tag 'Player' touch this object.
[RequireComponent(typeof(Collider2D))]
public class SignalOnTouch : MonoBehaviour
{
    [Tooltip("The UnityEvent to run when we collide.")]
    public UnityEvent onTouch;

    [Tooltip("If true, attempts to play an AudioSource when we collide.")]
    public bool playAudioOnTouch = true;

    // When we enter a trigger area, call SendSignal.
    public void OnTriggerEnter2D(Collider2D other)
    {
        _sendSignal(other.gameObject);
    }

    // When we collide with this object, call SendSignal.
    public void OnCollisionEnter2D(Collision2D other)
    {
        _sendSignal(other.gameObject);
    }

    // Checks to see if this object was tagged as Player, and invoke
    // the UnityEvent if it was.
    private void _sendSignal(GameObject objectThatHit)
    {
        // Was this object tagged 'Player'?
        if (!objectThatHit.CompareTag("Player")) return;
        
        // If we should play a sound, attempt to play it.
        if (playAudioOnTouch)
        {
            var audioSource = GetComponent<AudioSource>();
                
            // If we have audio clip and its parent is active, play a sound.
            if (audioSource && audioSource.gameObject.activeInHierarchy)
            {
                audioSource.Play();
            }
        }
            
        // Invoke the event.
        onTouch.Invoke();
    }
}
