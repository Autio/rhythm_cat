using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    // Behaviour for the notes coming down the track
    GameManager gm;

    public bool canBePressed;
    public bool beingPressed;
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
        // Long notes only. Regular notes currently handled by the ButtonController
        if (!gm.levelEnd && isLong)
        {

            if (Input.GetKeyDown(keyToPress))
            {
                HitNote();
            }  

            if(beingPressed)
            {
                LongNoteHeld();
            }


            if (Input.GetKeyUp(keyToPress) && isLong)
            {

                gm.cats[3].GetComponent<Cat>().RegularAnim();

            }
        }
    }

    public void HitNote()
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
                beingPressed = true;
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

                if (yPosition < (buttonY - 0.37f) || yPosition > (buttonY + 0.37f))
                    {
                        GameManager.instance.NormalHit();
                    }
                    else if (yPosition < (buttonY - 0.17f) || yPosition > (buttonY + 0.17f))
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

                GameManager.instance.ActivateNoteHitParticles(thisNoteType, isLong);
                GameManager.instance.SendCatNoteParticle(thisNoteType);
                // If it's not a long note, move the note up
                if (!isLong)
                {
                    LevelLoader.instance.NextNote();
                    GetComponent<SpriteRenderer>().enabled = false;
                    Destroy(this.gameObject, 3f);

                }
                else
                {
                    // Make the cat grow  FIXME: Hacky
                    if (!gm.cats[3].GetComponent<Cat>().growing)
                    {
                        gm.cats[3].GetComponent<Cat>().LongNoteAnim();
                    }
                    }
                // play the effect on the button

                }
            }


    }

    public void LongNoteHeld()
    {

        if (canBePressed)
        {
            if (isLong && hit)
            {
                longNoteScoreTicker -= Time.deltaTime;
                if (longNoteScoreTicker < 0)
                {
                    // If a long note is in the right collider area keep incrementing score
                    Debug.Log("Hitting long note!");
                    //GameManager.instance.ActivateNoteHitParticles(thisNoteType);
                    GameManager.instance.LongNoteHit();
                    longNoteScoreTicker = 0.3f;
                }
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
            // Generate the next note from the level map
            LevelLoader.instance.NextNote();

            // This note can no longer be pressed
            canBePressed = false;
            beingPressed = false;

            // What happens if the note passes the button but wasn't hit at all 
            // The note is missed
            if (!hit)
            {
                if (this.transform.tag != "LongNote" && this.transform.tag != "LongNoteTrigger")
                {
                    Debug.Log("Note missed and my tag is " + transform.tag);
                    Debug.Log("Note missed and my name is " + transform.name);
                    GameManager.instance.NoteMissed();

                    // Display the missed text above this button
                    GameObject g = Instantiate(missedText, new Vector3(transform.position.x, 2.55f, 0), missedText.transform.rotation) as GameObject;
                    g.GetComponent<PopupText>().dir = -1;
                    g.GetComponent<PopupText>().speed = 2f;

                    GameManager.instance.ActivateNoteMissedParticles(thisNoteType);

                }


            }
            else
            {
                Debug.Log("Long note exited");

            }
        }
    }
}
