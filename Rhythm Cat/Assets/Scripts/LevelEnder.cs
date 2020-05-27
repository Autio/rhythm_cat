using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnder : MonoBehaviour
{
    // Just used to end the level
    // Put this on an object that gets scrolled alongside the notes, after them and once
    // it hits the buttons, the level ends 
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered a trigger");
        if (other.tag == "Activator")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().EndLevel();
            Destroy(this.gameObject);
        }
    }
}
