using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;
    [SerializeField] private GameObject pauseMenu;

    public bool IsPaused { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        PlayerInput inputAction = GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerInput>();
        inputAction.SwitchCurrentActionMap("UI");

        pauseMenu.SetActive(true);
        GameObject resumeButton = pauseMenu.transform.GetChild(0).gameObject;
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void Unpause()
    {
        IsPaused = false;
        Time.timeScale = 1.0f;

        PlayerInput inputAction = GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerInput>();
        inputAction.SwitchCurrentActionMap("Player");

        pauseMenu.SetActive(false);
    }

    public void OnMainMenu()
    {
        Unpause(); 
        GameManager.Instance.ChangeScene("Main Menu");
    }

    public void OnQuitGame()
    {
        Debug.Log("Exiting Game");
#if UNITY_EDITOR
        // If running in the Unity Editor, stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build, quit the application
        Application.Quit();
#endif
    }

}
