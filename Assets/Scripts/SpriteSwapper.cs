using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Swaps out a sprite for another.
public class SpriteSwapper : MonoBehaviour
{
    [Tooltip("The sprite that should be displayed.")]
    public Sprite spriteToUse;

    [Tooltip("The sprite renderer that should use the new sprite.")]
    public SpriteRenderer spriteRenderer;

    private Sprite _originalSprite;

    // Swaps out the sprite.
    public void SwapSprite()
    {
        // If this sprite is different to the current sprite...
        if (spriteToUse != spriteRenderer.sprite)
        {
            // Store the previous one to original sprite.
            _originalSprite = spriteRenderer.sprite;

            // Make the sprite renderer use the new sprite.
            spriteRenderer.sprite = spriteToUse;
        }
    }
    
    // Reverts back to the old sprite.
    public void ResetSprite()
    {
        // If we have a previous sprite...
        if (_originalSprite != null)
        {
            // make the sprite renderer use it.
            spriteRenderer.sprite = _originalSprite;
        }
    }
}
