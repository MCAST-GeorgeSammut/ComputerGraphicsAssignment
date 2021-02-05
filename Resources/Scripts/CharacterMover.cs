using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    GameObject playerObject;

    [SerializeField]
    public float movementSpeed = 0f; 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            this.gameObject.transform.position+= new Vector3(0f, 0f, movementSpeed) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.gameObject.transform.position -= new Vector3(movementSpeed, 0f, 0f) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            this.gameObject.transform.position -= new Vector3(0f, 0f, movementSpeed) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.gameObject.transform.position += new Vector3(movementSpeed, 0f, 0f) * Time.deltaTime;
        }
    }
}
