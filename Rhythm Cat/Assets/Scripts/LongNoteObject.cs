using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNoteObject : MonoBehaviour
{

    public bool canBePressed;
    public bool hit = false;

    // Check when the long note starter collider is on top of the button and when it leaves
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Activator" && !hit)
        {
            canBePressed = false;
            GameManager.instance.NoteMissed();
        }
    }
}
