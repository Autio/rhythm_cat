using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer sr;
    public Sprite defaultImage;
    public Sprite pressedImage;
    bool notePresent = false;

    public KeyCode keyToPress;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(keyToPress))
        {
            sr.sprite = pressedImage;
            // Check whether there's a note under
            if (!notePresent)
            {
                // Tried to hit a note but missed
                GameObject.Find("GameManager").GetComponent<GameManager>().NoteMissed();
            }
               
        }

        if(Input.GetKeyUp(keyToPress))
        {
            sr.sprite = defaultImage;
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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Note")
        {
            notePresent = false;
            
        }
    }
}
