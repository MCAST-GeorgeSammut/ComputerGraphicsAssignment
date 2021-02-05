using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarCollisionBehaviour : MonoBehaviour
{
    void OnTriggerEnter(Collider collision)
    {
        //Behaviour for colliding with finishing line and switching scenes
        if (collision.gameObject.CompareTag("finishLine"))
        {
            Debug.Log("FINISHED LAP");

            if (SceneManager.GetActiveScene().name == "Level1")
            {
                SceneManager.LoadScene("Level2");
            }

            else if (SceneManager.GetActiveScene().name == "Level2")
            {
                SceneManager.LoadScene("Level3");
            }

            else if (SceneManager.GetActiveScene().name == "Level3")
            {
                Debug.Log("LEVEL 3 COMPLETE. YOU MAY NOW EXIT");
                Application.Quit();
            }
        }
        
        
    }
}
