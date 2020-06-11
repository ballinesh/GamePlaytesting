using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class LevelController : MonoBehaviour
{
    public string transitionLevelName;
    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.CompareTag("Player"))
        {
            //Loading level with build index
            SceneManager.LoadScene(1);

            //Loading level with scene name
            //SceneManager.LoadScene(transitionLevelName);

            //Restart  lvl
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
