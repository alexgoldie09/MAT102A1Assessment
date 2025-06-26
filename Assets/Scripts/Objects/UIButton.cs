using UnityEngine;

/*
 * UIButton.cs
 * -------------------
 * This script serves as a UI bridge component for buttons across various scenes.
 *
 * Tasks:
 *  - Acts as an intermediary between Unity's Button OnClick() UI system and the GameManager singleton.
 *  - Provides scene-agnostic access to GameManager functionality such as scene loading and quitting.
 *
 * Extras:
 *  - Required because Unity UI's OnClick() cannot directly invoke singleton/static methods.
 *  - Helps keep UI logic modular and decoupled from core game management.
 */

public class UIButton : MonoBehaviour
{
    // Loads a scene by using the GameManager singleton.
    public void LoadScene()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoadScene01();
    }

    // Triggers the game quit process through the GameManager singleton.
    public void QuitGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }

    public void ReturnMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenu();
    }
}
