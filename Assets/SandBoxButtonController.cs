using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SandboxButtonController : MonoBehaviour
{
    private Button sandbox;

    // Start is called before the first frame update
    void Start()
    {
        sandbox = GetComponent<Button>();
        sandbox.Select();
        if (sandbox != null)
        {
            sandbox.onClick.AddListener(OnSandboxClicked);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSandboxClicked()
    {
        Debug.Log("Sandbox Clicked");
        LoadScene("RonaldsToybox");
    }

    private void LoadScene(string sceneName)
    {
        GameManager.Instance.ChangeScene(sceneName);
    }
}
