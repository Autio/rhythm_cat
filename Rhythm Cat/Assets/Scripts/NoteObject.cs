using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    // Behaviour for the notes coming down the track
    GameManager gm;

    public bool canBePressed;
    public bool hit = false;
    public bool isLong = false;
       
    public KeyCode keyToPress;
    public enum noteTypes { blue, red, yellow, white }; // All possible note types. Change to black, gray, yellow, white?
    public noteTypes thisNoteType = noteTypes.blue;

    float longNoteScoreTicker = .3f; // every tick add score for held note

    GameObject longNoteStartTrigger;

    public GameObject perfectText;
    public GameObject goodText;
    public GameObject missedText;

    float buttonY;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        buttonY = GameManager.buttonY; 

        // If the note is long, there's a separate trigger in a child object
        // This governs the window of opportunity to start hitting the note
        if (isLong)
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
        // If the game level is active
        if (!gm.levelEnd)
        {
            if (Input.GetKeyDown(keyToPress))
            {
                // Bail out since the note has already been hit
                if (hit)
                {
                    return;
                }
                else
                {
                    if (canBePressed)
                    {
                        float yPosition = transform.position.y;
                        // If this is a long note, only the start trigger area should count
                        if (isLong && !longNoteStartTrigger.GetComponent<LongNoteObject>().hit)
                        {
                            yPosition = longNoteStartTrigger.transform.position.y;
                        }
                        if (isLong)
                        {
                            if (longNoteStartTrigger.GetComponent<LongNoteObject>().canBePressed == false)
                            {
                                return;
                            }
                        }

                        // Behaviour for regular notes
                        // Hit quality on the basis of the distance from 0
                        // IMPORTANT: Keep the ideal hit point at y = 0

                        if (yPosition > buttonY + 0.35f || yPosition < buttonY - 0.35f)
                        {
                            GameManager.instance.NormalHit();
                        }
                        else if (yPosition > buttonY + 0.12f || yPosition < buttonY - 0.12f)
                        {
                            GameManager.instance.GoodHit();

                            // Instantiate the relevant text above this note at the right position
                            GameObject g = Instantiate(goodText, new Vector3(transform.position.x, 2.55f, 0), goodText.transform.rotation) as GameObject;
                            //g.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

                        }
                        else 
                        {
                            GameManager.instance.PerfectHit();
                            Instantiate(perfectText, new Vector3(transform.position.x, 2.55f, 0), perfectText.transform.rotation);

                        }
                        hit = true;

                        // If it's not a long note, make the note invisible
                        if (!isLong)
                        {
                            GetComponent<SpriteRenderer>().enabled = false;
                        } else
                        {
                            // Make the cat grow  FIXME: Hacky
                            gm.cats[3].GetComponent<Cat>().LongNoteAnim();
                        }
                        // play the effect on the button
                        GameManager.instance.ActivateNoteHitParticles(thisNoteType);
                        GameManager.instance.SendCatNoteParticle(thisNoteType);
                    }
                }

            }

            if (canBePressed)
            {
                if (Input.GetKey(keyToPress) && isLong && hit)
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

            if(Input.GetKeyUp(keyToPress) && isLong)
            {
                
                gm.cats[3].GetComponent<Cat>().RegularAnim();

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
        if (other.tag == "Activator")
        {
            canBePressed = false;
            if (!hit)
            {
                if (this.transform.tag != "LongNote" && this.transform.tag != "LongNoteTrigger")
                {
                    Debug.Log("Note missed and my tag is " + transform.tag);
                    Debug.Log("Note missed and my name is " + transform.name);
                    GameManager.instance.NoteMissed();

                    // Display the missed text above this button
                    GameObject g = Instantiate(missedText, new Vector3(transform.position.x, 2.55f, 0), missedText.transform.rotation) as GameObject;

                }
             

            }
            else
            {
                Debug.Log("Long note exited");

            }
        }
    }
}
