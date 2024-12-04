using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFoxTrigger : MonoBehaviour
{
    //[SerializeField] private string sceneDestination;

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("DestroyablePlant").Length == 0)
        {
            Destroy(GameObject.Find("cage"));
        }
    }
}
