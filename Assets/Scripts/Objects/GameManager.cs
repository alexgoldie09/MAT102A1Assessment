using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Tooltip("Name of the scene to reload on game over.")]
    public string sceneToReload = "CustomSceneMaths";

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Call this to trigger a delayed scene reload (e.g., after death).
    /// </summary>
    public void TriggerGameOver(float delay = 5f)
    {
        StartCoroutine(ReloadSceneAfterDelay(delay));
    }

    private IEnumerator ReloadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToReload);
    }
}
