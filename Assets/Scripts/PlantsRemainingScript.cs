using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantsRemainingScript : MonoBehaviour
{
    public TextMeshProUGUI remainingPlants;

    private void Start()
    {
        remainingPlants = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        remainingPlants.text = "Plants Remaining: " + GameObject.FindGameObjectsWithTag("DestroyablePlant").Length;
    }
}
