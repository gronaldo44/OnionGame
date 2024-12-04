using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndScreenManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("MainMenu"));
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void OnMainMenu()
    {
        GameManager.Instance.ChangeScene("Main Menu");
    }
}
