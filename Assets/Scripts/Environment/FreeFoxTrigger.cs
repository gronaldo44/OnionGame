using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FreeFoxTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset newFile;

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("DestroyablePlant").Length == 0)
        {
            GameObject.Find("Selene").GetComponent<DialogueTrigger>().textFile = newFile;
            Destroy(GameObject.Find("cage"));
        }
    }
}
