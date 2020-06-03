using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int health = 50;
    // Start is called before the first frame update
    public GameObject deathEffect;
    public void takeDamage(int damage)
    {
        health-= damage;

        if(health <= 0)
        {
            die();
        }
    }

    void die()
    {
         Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
