using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * GameManager.cs
 * --------------
 * This script switches between scenes and triggers game over.
 *
 * Tasks:
 *  - Implements a persistent singleton pattern using `DontDestroyOnLoad`.
 *  - Provides a `TriggerGameOver` method to initiate a scene reload after a timed delay.
 *  - It uses methods that are accessed via buttons in Unity.
 *   + Methods load scene using Unity's SceneManager.
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

    // LoadScene01() loads in this specific scene (Name has to be spelt correctly).
    public void LoadScene01()
    {
        SceneManager.LoadScene(sceneToReload);
    }

    // ReturnToMainMenu() loads in this specific scene (Name has to be spelt correctly).
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // QuitGame() ends the game session.
    public void QuitGame()
    {
#if UNITY_EDITOR
        // Stop playing in the editor.
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // TriggerGameOver() triggers a delayed scene reload (e.g., after death).
    public void TriggerGameOver(float delay = 5f)
    {
        StartCoroutine(ReloadSceneAfterDelay(delay));
    }

    // ReloadSceneAfterDelay() coroutine delays before reloading the specified scene.
    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene01();
    }
}
