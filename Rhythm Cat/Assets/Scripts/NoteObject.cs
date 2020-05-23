using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public bool hit = false;
    public bool isLong = false;
    

    public KeyCode keyToPress;
    public enum noteTypes { blue, red, yellow, white };
    public noteTypes thisNoteType = noteTypes.blue;
    float longNoteScoreTicker = .3f; // every tick add score for held note

    GameObject longNoteStartTrigger;

    public GameObject perfectText;
    public GameObject goodText;

    // Start is called before the first frame update
    void Start()
    {
        if(isLong)
        {
            longNoteStartTrigger = this.transform.Find("StartTrigger").gameObject;

        } else
        {
            Debug.LogError("Could not find the child trigger object of the long note. This is needed to give the window of opportunity to hit the note at its beginning.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
            if (hit)
            {
                return;
            }
            else 
            {
                if (canBePressed)
                {
                    float yPosition = transform.position.y;
                    if(isLong)
                    {
                        canBePressed = longNoteStartTrigger.GetComponent<LongNoteObject>().canBePressed;
                    }
                    // If this is a long note, only the start trigger area should count
                    if (isLong && !longNoteStartTrigger.GetComponent<LongNoteObject>().hit)
                    {
                        yPosition = longNoteStartTrigger.transform.position.y;
                    }

                    // Hit quality on the basis of the distance from 0
                    if (Mathf.Abs(yPosition) > 0.25f)
                    {
                        GameManager.instance.NormalHit();
                    }
                    else if (Mathf.Abs(yPosition) > .05f)
                    {
                        GameManager.instance.GoodHit();
                        // Instantiate the relevant text above this note at the right position
                        Instantiate(goodText, new Vector3(transform.position.x, 6.85f, 0), goodText.transform.rotation);

                    }
                    else
                    {
                        GameManager.instance.PerfectHit();
                        Instantiate(perfectText, new Vector3(transform.position.x, 6.85f, 0), perfectText.transform.rotation);

                    }
                    hit = true;

                    // If it's not a long note, make the note invisible
                    if (!isLong)
                    {
                        GetComponent<SpriteRenderer>().enabled = false;
                    }
                    // play the effect on the button
                    GameManager.instance.ActivateNoteHitParticles(thisNoteType);
                }
            }

        }

        if(Input.GetKey(keyToPress) && isLong && canBePressed && hit)
        {
            longNoteScoreTicker -= Time.deltaTime;
            if (longNoteScoreTicker < 0)
            {
                // If a long note is in the right collider area keep incrementing score
                Debug.Log("Hitting long note!");
                GameManager.instance.ActivateNoteHitParticles(thisNoteType);
                GameManager.instance.LongNoteHit();
                longNoteScoreTicker = 0.3f;
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
