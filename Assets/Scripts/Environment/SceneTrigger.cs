using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string sceneDestination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameManager.ChangeScene(sceneDestination);
    }
}
