using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    //Health Bar Variables
    public Slider healthBar;
    float startingHealth;
    [SerializeField] private float playerHealth;

    //TEMPORARY PARAMS FOR TESTING
    //private float interval = 3f;
    ////private float timer = 0f;

    void Start()
    {
        //TEMPORARY!!!
        setHealth(10);

    }

    void Update()
    {
        healthBar.value = playerHealth;

        //FOR TESTING TEMPORARY!!!
        //timer += Time.deltaTime;

        //if (timer >= interval)
        //{
        //    timer -= interval;
        //    playerHealth--;
        //    setHealth(playerHealth);
        //}
    }

    private void setHealth(float health)
    {
        playerHealth = health;
        Debug.Log(playerHealth);
    }
}
