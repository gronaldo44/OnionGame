using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string sceneDestination;

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("DestroyablePlant").Length == 0)
        {
            GameManager.Instance.ChangeScene(sceneDestination);
        } 
    }
}
