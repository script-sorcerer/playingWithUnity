using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BodyPart : MonoBehaviour
{
    [Tooltip("The sprite to use when ApplyDamageSprite is called with damage type 'slicing'.")]
    public Sprite detachedSprite;
    
    [Tooltip("The sprite to use when ApplyDamageSprite is called with damage type 'burn'.")]
    public Sprite burnedSprite;

    [Tooltip("Represents the position and rotation that a blood fountain will appear at on the main body.")]
    public Transform bloodFountainOrigin;

    // If true, this object will remove its collision, joins and rigidbody
    // when it comes to rest.
    private bool _detached = true;

    // Decouple this object from the parent, and flag it as needing
    // physics removal.
    public void Detach()
    {
        _detached = true;
        tag = "Untagged";
        
        transform.SetParent(null, true);
    }
    
    // Every frame, if we're detached, remove physics if the rigidbody is falling.
    // This means that detached body parts will never get in the way of the gnome.
    void Update()
    {
        // If we're not detaching, do nothing.
        if (_detached == false)
        {
            return;
        }

        var rigidBody = GetComponent<Rigidbody2D>();
        // Is our rigidbody stop falling?
        if (rigidBody.IsSleeping())
        {
            // If so, destroy all joints...
            foreach (var joint in GetComponentsInChildren<Joint2D>())
            {
                Destroy(joint);
            }
            
            // ...rigidbodies...
            foreach (var body in GetComponentsInChildren<Rigidbody2D>())
            {
                Destroy(body);
            }
            
            // ...and the collider.
            foreach (var coll in GetComponentsInChildren<Collider2D>())
            {
                Destroy(coll);
            }
            
            // Finally, remove this script.
            Destroy(this);
        }
    }

    // Changes the sprite for this part based on taken damage kind.
    public void ApplyDamageSprite(Gnome.damageType damageType)
    {
        Sprite spriteToUse = null;

        switch (damageType)
        {
            case Gnome.damageType.Burning:
                spriteToUse = burnedSprite;
                break;
            case Gnome.damageType.Slicing:
                spriteToUse = detachedSprite;
                break;
        }

        if (spriteToUse != null)
        {
            GetComponent<SpriteRenderer>().sprite = spriteToUse;
        }
    }
    void Start()
    {
        
    }
}
