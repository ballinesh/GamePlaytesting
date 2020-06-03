using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 10;
    public Rigidbody2D rigidBody;
    public GameObject bulletImpact;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody.velocity = transform.right * speed;    
    }
    
    private void OnTriggerEnter2D(Collider2D hitInfo) {
        {
            //This will be used once I decide to craete an enemy object lol
            Enemy enemy = hitInfo.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.takeDamage(damage);
            } 
            Destroy(gameObject);
            Instantiate(bulletImpact,transform.position, transform.rotation);
        }
    }
}
