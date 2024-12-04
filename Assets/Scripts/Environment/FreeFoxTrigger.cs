using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeFoxTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset newText;

    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("DestroyablePlant").Length == 0)
        {
            GameObject.Find("Selene").GetComponent<DialogueTrigger>().textFile = newText;
            Destroy(GameObject.Find("cage"));
        }
    }
}
