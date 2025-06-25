using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/*
 * GameManager.cs
 * --------------
 * This script defines a basic singleton GameManager responsible for reloading the scene
 * after a game over event, typically after the player's spaceship has been destroyed.
 *
 * Tasks:
 *  - Implements a persistent singleton pattern using `DontDestroyOnLoad`.
 *  - Provides a `TriggerGameOver` method to initiate a scene reload after a timed delay.
 *  - Useful in decoupling gameplay logic from scene management for clean modularity.
 *
 * Extras:
 *  - The reload delay allows VFX or transition animations to complete before resetting.
 *  - Centralized game state control makes future extensibility (e.g., score tracking or level transitions) easier.
 */

public class GameManager : MonoBehaviour
{
    // Singleton instance accessible from any script
    public static GameManager Instance { get; private set; }

    // Scene name to reload after ship destruction
    public string sceneToReload = "CustomSceneMaths";

    /*
     * Awake() initialises before all instances to save this object as a Singleton. 
    */
    void Awake()
    {
        // Ensures only one instance exists; destroys duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Persist across scene loads
        DontDestroyOnLoad(gameObject);
    }

    /*
     * TriggerGameOver() triggers a delayed scene reload (e.g., after death).
    */
    public void TriggerGameOver(float delay = 5f)
    {
        StartCoroutine(ReloadSceneAfterDelay(delay));
    }

    /*
     * ReloadSceneAfterDelay() coroutine delays before reloading the specified scene.
    */
    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToReload);
    }
}
