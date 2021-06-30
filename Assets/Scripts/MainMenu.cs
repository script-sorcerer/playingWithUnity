using UnityEngine.SceneManagement;
using UnityEngine;

// Controls over main menu.
public class MainMenu : MonoBehaviour
{
    [Tooltip("The name of a scene with the game process.")]
    public string sceneToLoad;

    [Tooltip("The UI component with text 'Loading...'.")]
    public RectTransform loadingOverlay;
    
    // Proceed scene loading in the background. Use it to switch the scene.
    private AsyncOperation _sceneLoadingOperation;
    
    // Start is called before the first frame update.
    private void Start()
    {
        // Hide "Loading..." screen.
        loadingOverlay.gameObject.SetActive(false);
        
        // Kick off loading of the scene...
        _sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        
        // but with no switching to the new scene unless we not ready.
        _sceneLoadingOperation.allowSceneActivation = false;
    }

    // Calls when "New Game" button touches.
    public void LoadScene()
    {
        // Make "Loading..." scene be visible.
        loadingOverlay.gameObject.SetActive(true);
        
        // Tell the loading operation to switch scene on complete loading.
        _sceneLoadingOperation.allowSceneActivation = true;
    }
}
