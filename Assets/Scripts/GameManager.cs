using System.Collections;
using UnityEngine;

// Manages the game state.
public class GameManager : Singleton<GameManager>
{
    [Tooltip("The location where the gnome should appear.")]
    public GameObject startingPoint;

    [Tooltip("The rope object, which lowers and raises the gnome.")]
    public Rope rope;

    [Tooltip("The follow script, which will follow the gnome.")]
    public CameraFollow cameraFollow;

    [Tooltip("The prefab to create a new gnome.")]
    public GameObject gnomePrefab;

    [Tooltip("The UI component that contains the 'restart' and 'resume' buttons.")]
    public RectTransform mainMenu;

    [Tooltip("The UI component that contains the 'up', 'down' and 'menu' buttons.")]
    public RectTransform gameplayMenu;

    [Tooltip("The UI component that contains the 'you win!' screen.")]
    public RectTransform gameOverMenu;
    
    [Tooltip("How long to wait after dying before creating a new gnome.")]
    public float delayAfterDeath = 1.0f;

    [Tooltip("The sound to play when the gnome dies.")]
    public AudioClip gnomeDiedSound;

    [Tooltip("The sound to play when the game is won.")]
    public AudioClip gameOverSound;
    
    // If true, ignore all damage (but still show damage effects).
    // The 'get; set;' make this a property, to make it show
    // up in the list of methods in the Inspector for Unity Events.
    public bool gnomeInvincible { get; set; }
    
    // The alive gnome.
    private Gnome _currentGnome;
    
    // Start is called before the first frame update.
    void Start()
    {
        // When the game starts, call Reset to set up the gnome.
        Reset();
    }

    // Reset the entire game.
    public void Reset()
    {
        // Turn off the menus, turn on the gameplay UI.
        if (gameOverMenu)
        {
            gameOverMenu.gameObject.SetActive(false);
        }
        if (mainMenu)
        {
            mainMenu.gameObject.SetActive(false);
        }

        if (gameplayMenu)
        {
            gameplayMenu.gameObject.SetActive(true);
        }
        
        // Find all Resettable components and tell them to reset.
        var resetObjects = FindObjectsOfType<Resettable>();
        foreach (var resetObject in resetObjects)
        {
            resetObject.Reset();
        }
        
        // Make a new gnome.
        CreateNewGnome();
        
        // Un-pause the game.
        Time.timeScale = 1.0f;
    }

    void CreateNewGnome()
    {
        // Destroy the current gnome, if there is one.
        RemoveGnome();
        
        // Create a new gnome object, and make it be our currentGnome.
        var newGnome = Instantiate(gnomePrefab, startingPoint.transform.position, Quaternion.identity);
        _currentGnome = newGnome.GetComponent<Gnome>();
        
        // Make the rope visible.
        rope.gameObject.SetActive(true);
        
        // Connect the rope's trailing end to whichever rigidbody the Gnome object wants (e.g. his foot).
        rope.connectedObject = _currentGnome.ropeBody;

        // Sets the rope size to the default.
        rope.ResetLength();
        
        // Tell the cameraFollow to start tracking the new game object.
        cameraFollow.target = _currentGnome.cameraFollowTarget;
    }

    void RemoveGnome()
    {
        // Do nothing if the gnome is invincible.
        if (gnomeInvincible)
        {
            return;
        }
        
        // Hide the rope.
        rope.gameObject.SetActive(false);
        
        // Stop tracking the gnome.
        cameraFollow.target = null;
        
        // If we have a current gnome, make that no longer be the player.
        if (_currentGnome != null)
        {
            // This gnome is no longer holding the treasure.
            _currentGnome.holdingTreasure = false;
            
            // Mark this object as not the player (so that colliders won't report when the object hits them).
            _currentGnome.gameObject.tag = "Untagged";
            
            // Find all objects with the 'Player' tag, and remove them.
            foreach (Transform child in _currentGnome.transform)
            {
                child.gameObject.tag = "Untagged";
            }
            
            // Mark ourselves as not currently having a gnome
            _currentGnome = null;
        }
    }

    // Kills the gnome.
    void KillGnome(Gnome.DamageType damageType)
    {
        // If we have an audio source, play 'gnome died' sound.
        var audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.PlayOneShot(gnomeDiedSound);
        }
        
        // Show the damage effect.
        _currentGnome.showDamageEffect(damageType);
        
        // If we're not invincible, reset the game and make the gnome not be the current player.
        if (gnomeInvincible == false)
        {
            // Tell the gnome that it died.
            _currentGnome.DestroyGnome(damageType);
            
            // Remove the gnome.
            RemoveGnome();
            
            // Reset the game.
            StartCoroutine(ResetAfterDelay());
        }
    }

    // Called when gnome dies.
    IEnumerator ResetAfterDelay()
    {
        // Wait for delayAfterDeath seconds, then call Reset.
        yield return new WaitForSeconds(delayAfterDeath);
        Reset();
    }

    // Called when the player touches a trap.
    public void TrapTouched()
    {
        KillGnome(Gnome.DamageType.Slicing);
    }

    // Called when the player touches a fire trap.
    public void FireTrapTouched()
    {
        KillGnome(Gnome.DamageType.Burning);
    }

    // Called when the gnome picks up the treasure.
    public void TreasureCollected()
    {
        // Tell the currentGnome that it should have the treasure.
        _currentGnome.holdingTreasure = true;
    }
    
    // Called when the player touches th exit.
    public void ExitReached()
    {
        // If we have a player, and the player holding the treasure, game over!
        if (_currentGnome != null && _currentGnome.holdingTreasure)
        {
            // If we have an audio source, play the game over sound.
            var audioSource = GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.PlayOneShot(gameOverSound);
            }
            
            // Pause the game
            Time.timeScale = 0.0f;
            
            // Turn off the gameplay menu, and turn on the game over screen!
            if (gameOverMenu)
            {
                gameOverMenu.gameObject.SetActive(true);
            }

            if (gameplayMenu)
            {
                gameplayMenu.gameObject.SetActive(false);
            }
        }
    }
    
    // Called when the Menu button is tapped, and when the Resume game is tapped
    public void SetPaused(bool paused)
    {
        // If we're paused, stop time and enable the menu (and disable 
        // the game overlay)
        if (paused)
        {
            Time.timeScale = 0.0f;
            mainMenu.gameObject.SetActive(true);
            gameplayMenu.gameObject.SetActive(false);
        }
        else
        {
            // If we're not paused, resume time and disable the 
            // menu (and enable the game overlay)
            Time.timeScale = 1.0f;
            mainMenu.gameObject.SetActive(false);
            gameplayMenu.gameObject.SetActive(true);
        }
    }
    
    // Called when the Restart button is tapped.
    public void RestartGame()
    {
        // Immediately remove the gnome(instead of killing it)
        Destroy(_currentGnome.gameObject);
        _currentGnome = null;
        
        // Now reset the game to create a new gnome.
        Reset();
    }
}
