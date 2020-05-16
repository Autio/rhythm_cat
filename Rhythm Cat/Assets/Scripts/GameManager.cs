using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    public AudioSource music;

    public bool startPlaying;

    public BeatScroller bs;

    public static GameManager instance;

    public int currentScore;
    public int scorePerNote = 100;
    public int sequence = 0; // Sequence of correctly hit notes
    public int multiplier = 1;
    public int multiplierThreshold = 10; // How many hits do you need to get in sequence before the multiplier is bumped up

    public GameObject scoreText;
    public GameObject multiplierText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(!startPlaying)
        {
            if(Input.anyKeyDown)
            {
                startPlaying = true;
                bs.hasStarted = true;

                music.Play();
            }
        }
    }

    public void NoteHit()
    {
        Debug.Log("Note hit");

        currentScore += scorePerNote;
        try
        {
            scoreText.GetComponent<TMP_Text>().text = currentScore.ToString();
            sequence++;

            // Check if it is time to bump up the multiplier
            if (CheckMultiplier())
            {
                multiplier++;
                multiplierText.GetComponent<TMP_Text>().text = multiplier.ToString();
            }

        }
        catch
        {
            Debug.Log("Score text couldn't be changed.");
        }
    }

    public void NoteMissed()
    {
        Debug.Log("Note missed");
        // Reset sequence if you miss a note
        sequence = 0;
    }

    bool CheckMultiplier()
    {
        if(multiplier % multiplierThreshold == 0)
        {
            return true;
        }
        return false;
    }

}
