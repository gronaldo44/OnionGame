using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class endScreenTrigger : MonoBehaviour
{

    [SerializeField] private string sceneDestination;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent<PlayerController>(out var player)) //Check if player is in range
        {
            Debug.Log("colliding with fox");
            GameManager.Instance.ChangeScene(sceneDestination);
        }

 
    }

}
