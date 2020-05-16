using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public bool hit = false;
    public KeyCode keyToPress;
    public enum noteTypes { blue, red, yellow, white };
    public noteTypes thisNoteType = noteTypes.blue;



    public GameObject perfectText;
    public GameObject goodText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
            if(canBePressed && !hit)
            {
                // Hit quality on the basis of the distance from 0
                if(Mathf.Abs(transform.position.y) > 0.25f)
                {
                    GameManager.instance.NormalHit();
                } else if (Mathf.Abs(transform.position.y) > .05f)
                    {
                    GameManager.instance.GoodHit();
                    // Instantiate the relevant text above this note at the right position
                    Instantiate(goodText, new Vector3(transform.position.x, 6.85f, 0), goodText.transform.rotation);

                } else
                {
                    GameManager.instance.PerfectHit();
                    Instantiate(perfectText, new Vector3(transform.position.x, 6.85f, 0), perfectText.transform.rotation);

                }
                hit = true;
                // Make the note invisible
                GetComponent<SpriteRenderer>().enabled = false;

                // play the effect on the button
                GameManager.instance.ActivateNoteHitParticles(thisNoteType);
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Activator")
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
