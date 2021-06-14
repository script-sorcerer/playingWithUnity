using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnome : MonoBehaviour
{
    [Tooltip("The object that the camera should follow.")]
    public Transform cameraFollowTarget;

    public Rigidbody2D ropeBody;

    public Sprite armHoldingEmpty;

    public Sprite armHoldingTreasure;

    public SpriteRenderer holdingArm;

    public GameObject deathPrefab;

    public GameObject flameDeathPrefab;

    public GameObject ghostPrefab;

    public GameObject bloodFountainPrefab;

    public float delayBeforeRemoving = 3.0f;

    public float delayBeforeReleasingGhost = 0.25f;

    private bool _dead = false;

    private bool _holdingTreasure = false;

    public bool holdingTreasure
    {
        get => _holdingTreasure;
        set
        {
            if (_dead)
            {
                return;
            }

            _holdingTreasure = value;

            if (holdingArm == null) return;
            holdingArm.sprite = _holdingTreasure ? armHoldingTreasure : armHoldingEmpty;
        }
    }

    public enum DamageType
    {
        Slicing,
        Burning
    }

    public void showDamageEffect(DamageType type)
    {
        switch (type)
        {
            case DamageType.Burning:
                if (flameDeathPrefab != null)
                {
                    Instantiate(flameDeathPrefab, cameraFollowTarget.position, cameraFollowTarget.rotation);
                }
                break;
            case DamageType.Slicing:
                if (deathPrefab != null)
                {
                    Instantiate(deathPrefab, cameraFollowTarget.position, cameraFollowTarget.rotation);
                }
                break;
        }
    }

    public void DestroyGnome(DamageType type)
    {
        holdingTreasure = false;
        _dead = true;

        // Find all child objects, and randomly disconnect their joints.
        foreach (var bodyPart in GetComponentsInChildren<BodyPart>())
        {
            switch (type)
            {
                case DamageType.Burning:
                    // 1 in 3 chance of burning.
                    var shouldBurn = Random.Range(0, 2) == 0;
                    if (shouldBurn)
                    {
                        bodyPart.ApplyDamageSprite(type);
                    }
                    break;
                case DamageType.Slicing:
                    // Slice damage always applies a damage sprite.
                    bodyPart.ApplyDamageSprite(type);
                    break;
            }
            
            // 1 in 3 chance of separating from body.
            var shouldDetach = Random.Range(0, 2) == 0;

            if (shouldDetach)
            {
                // Make this object remove its rigidbody and collider after it comes to rest.
                bodyPart.Detach();
                
                // If we're separating and damage type is Slicing -> add blood fountain.
                if (type == DamageType.Slicing)
                {
                    if (bodyPart.bloodFountainOrigin != null && bloodFountainPrefab != null)
                    {
                        // Attach a blood fountain for this detached part.
                        var fountain = Instantiate(bloodFountainPrefab, bodyPart.bloodFountainOrigin.position,
                            bodyPart.bloodFountainOrigin.rotation);
                        
                        fountain.transform.SetParent(cameraFollowTarget, false);
                    }
                }
                
                // Disconnect this object.
                var allJoints = bodyPart.GetComponentsInChildren<Joint2D>();
                foreach (var joint in allJoints)
                {
                    Destroy(joint);
                }
            }
        }
        
        // Add a RemoveAfterDelay component to this object.
        var remove = gameObject.AddComponent<RemoveAfterDelay>();
        remove.delay = delayBeforeRemoving;
        StartCoroutine(ReleaseGhost());
    }

    IEnumerator ReleaseGhost()
    {
        // No ghost prefab? Bail out.
        if (ghostPrefab == null)
        {
            yield break;
        }
        
        // Wait for delayBeforeReleasingGhost seconds.
        yield return new WaitForSeconds(delayBeforeReleasingGhost);
        
        // Add the ghost.
        Instantiate(ghostPrefab, transform.position, Quaternion.identity);
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
