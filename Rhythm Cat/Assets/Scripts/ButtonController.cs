using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonController : MonoBehaviour
{
    public bool allowMisses = true;
    private SpriteRenderer sr;
    public Sprite defaultImage;
    public Sprite pressedImage;
    bool notePresent = false;
    bool songStarted = false;
    bool longNote = false;

    public GameObject movingMouth;
    public float duration = .3f;
    public float scaleBoost = 1.5f;

    public KeyCode keyToPress;
    public GameObject catMouth;

    GameManager gm;

    public List<GameObject> notesUnderMe;

    [SerializeField]
    private ParticleSystem ps;


    // Start is called before the first frame update
    void Start()
    {
        notesUnderMe = new List<GameObject>();
        sr = GetComponent<SpriteRenderer>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void TouchButton()
    {

        PressButton();

        // Cycle through notes and trigger the relevant one also
        foreach(GameObject note in gm.notes)
        {
            if(note.GetComponent<NoteObject>().keyToPress == this.keyToPress)
            {
                note.GetComponent<NoteObject>().HitNote();
            }
        }
        foreach (GameObject longNote in gm.longNotes)
        {
            if (longNote.GetComponent<NoteObject>().keyToPress == this.keyToPress)
            {
                longNote.GetComponent<NoteObject>().HitNote();
            }
        }
    }

    public void PressButton()
    {
        MouthOpen();
        sr.sprite = pressedImage;
        bool noteUnder = false;
        foreach (GameObject g in notesUnderMe)
        {
            try
            {
                if (GetComponent<BoxCollider2D>().bounds.Intersects(g.GetComponent<CircleCollider2D>().bounds))
                {
                    noteUnder = true;
                }
            }
            catch
            {
                Debug.Log("Couldn't find overlap bounds");
            }
        }

        // Check whether there's a note under
        if (!noteUnder && songStarted && !longNote && allowMisses)
        {
            // Tried to hit a note but missed
            GameObject.Find("GameManager").GetComponent<GameManager>().NoteMissed();

        }
    }

    public void UnpressButton()
    {
        MouthClose();

        sr.sprite = defaultImage;

        // Cycle through notes and trigger the relevant one also
        foreach (GameObject note in gm.notes)
        {
            if (note.GetComponent<NoteObject>().keyToPress == this.keyToPress)
            {
                note.GetComponent<NoteObject>().beingPressed = false;
            }
        }
        foreach (GameObject longNote in gm.longNotes)
        {
            if (longNote.GetComponent<NoteObject>().keyToPress == this.keyToPress)
            {
                longNote.GetComponent<NoteObject>().beingPressed = false;
            }
        }

        gm.cats[3].GetComponent<Cat>().RegularAnim();
        if (longNote)
        {
            ps.Stop();
        }

    }

    // Update is called once per frame
    void Update()
    {

            if (Input.GetKeyDown(keyToPress))
            {
                PressButton();
            }

            if (Input.GetKeyUp(keyToPress))
            {
                UnpressButton();
            }
        
    }

    public void PulseButton()
    {
        // Make the button pulse large
        Sequence seq = DOTween.Sequence();
        seq.Append(this.GetComponent<Transform>().DOScale(new Vector3(2,2,2),.3f));
        seq.Append(this.GetComponent<Transform>().DOScale(new Vector3(1, 1, 1),.5f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Note")
        {
            notePresent = true;
            notesUnderMe.Add(other.gameObject);

        }
        if (other.tag == "LongNote")
        {
            longNote = true;
        }

        if (other.tag == "Start")
        {
            songStarted = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Note" )
        {
            notePresent = false;
            notesUnderMe.Remove(other.gameObject);

        }
        if (other.tag == "LongNote")
        {
            longNote = false;
            // End visual effect on button
            ps.Stop();
        }
    }

    public void MouthOpen()
    {
        // Make the mouth seem like it's singing

        Sequence seq = DOTween.Sequence();
        seq.Append(movingMouth.GetComponent<Transform>().DOScale(scaleBoost, duration / 2));
        //    seq.Append(movingMouth.GetComponent<Transform>().DOScale(1f, duration / 2));


        //Debug.Log("Mouth of cat " + keyToPress.ToString() + " Singing");
        //Sequence seq = DOTween.Sequence();
        //seq.Append(catMouth.GetComponent<Transform>().DOScale(12f, 1f));
        //seq.Append(catMouth.GetComponent<Transform>().DOScale(1f, .2f));
    }

    public void MouthClose()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(movingMouth.GetComponent<Transform>().DOScale(1f, duration / 2));
    }
}
