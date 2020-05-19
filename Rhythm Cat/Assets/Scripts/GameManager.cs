using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public AudioSource music;
    public GameObject gameScene;
    public GameObject startCanvas;
    public GameObject liveGameCanvas;
    public GameObject endCanvas;
    public GameObject curtain;
    public GameObject[] buttons;

    public bool startPlaying;
    bool levelEnd = false;
    bool startScreen = true;
    bool transition = false;

    public BeatScroller bs;

    public static GameManager instance;

    public int currentScore;
    public int scorePerNormalHit = 80;
    public int scorePerGoodHit = 100;
    public int scorePerPerfectHit = 120;
    public int sequence = 0; // Sequence of correctly hit notes
    public int multiplier = 1;
    public int multiplierThreshold = 10; // How many hits do you need to get in sequence before the multiplier is bumped up
    // Do we want an array of thresholds?
    public int[] multiplierThresholds = { 4, 8, 16, 32, 64, 128, 256, 512 };

    public int health = 40;
    private int maxHealth;

    public GameObject scoreText;
    public GameObject multiplierText;
    public GameObject sequenceText;
    public GameObject healthBar;

    public GameObject performanceText;
    public GameObject finalScoreText;
    public GameObject notesHitText;
    
    public GameObject[] buttonHitParticleEffects;

    int notesHit;
    int notesMissed;
    int totalNotes; // How many notes on the scene

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] notes = GameObject.FindGameObjectsWithTag("Note");
        totalNotes = notes.Length;
        Debug.Log("Total notes: " + totalNotes.ToString());

        gameScene.SetActive(false);
        startCanvas.SetActive(true);
        liveGameCanvas.SetActive(false);
        endCanvas.SetActive(false);

        instance = this;
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(!transition)
        {

        if(startScreen)
        {
            if (Input.anyKeyDown)
            {
                startScreen = false;
                // enable main game and hide the start screen
                // Start moving the curtains to the right 
                Sequence seq = DOTween.Sequence();
                seq.Append(curtain.GetComponent<Transform>().DOMoveX(-12.64f, 2f));
                transition = true;
                StartCoroutine(DrawCurtain());
                startCanvas.SetActive(false);
                gameScene.SetActive(true);

            }
        } else if (!startPlaying)
        {
            if(Input.anyKeyDown)
            {
                startPlaying = true;
                bs.hasStarted = true;

                music.Play();
            }
        } else if (levelEnd)
        {
            if(Input.anyKeyDown)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            
        }

        }

    }

    public void NoteHit()
    {
        Debug.Log("Note hit");
        HealthUp();
        notesHit++;

        // Visual effect on score object
        scoreText.GetComponent<GenericText>().PulseText();

        try
        {
            scoreText.GetComponent<TMP_Text>().text = currentScore.ToString();
            sequence++;
            sequenceText.GetComponent<TMP_Text>().text = sequence.ToString();


            // Check if it is time to bump up the multiplier
            if (CheckMultiplier())
            {
                multiplier++;
                multiplierText.GetComponent<GenericText>().PulseText();

                multiplierText.GetComponent<TMP_Text>().text = multiplier.ToString() +"X";
            }

        }
        catch
        {
            Debug.Log("Score text couldn't be changed.");
        }
    }

    public void NormalHit()
    {
       currentScore += scorePerNormalHit * multiplier;
       Debug.Log("Normal hit!");
       NoteHit();
    }

    public void GoodHit()
    {
        currentScore += scorePerGoodHit * multiplier;
        Debug.Log("Good hit!");
        NoteHit();

    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectHit * multiplier;
        Debug.Log("Perfect hit!");
        NoteHit();

    }

    public void NoteMissed()
    {
        Debug.Log("Note missed");
        HealthDown();
        // Reset sequence if you miss a note
        sequence = 0;
        notesMissed++;

    }

    void HealthDown()
    {
        health -= 4;
        UpdateHealthBar();
        if (health <= 0)
        {
            // Level lost
            EndLevel();

        }
    }
    void HealthUp()
    {
        if(health >= maxHealth)
        {
            return;
        }
        health += 2;
        health = Mathf.Clamp(health, 1, maxHealth);
        UpdateHealthBar();
        
    }

    bool CheckMultiplier()
    {
        if(sequence > multiplierThresholds[multiplier - 1])
        {
            return true;
        }
        return false;
    }

    void UpdateHealthBar()
    {
        healthBar.GetComponent<Slider>().value = (float)health/ (float)maxHealth;
    }

    public void ActivateNoteHitParticles(NoteObject.noteTypes n)
    {
        if(n == NoteObject.noteTypes.blue)
        {
            buttonHitParticleEffects[0].GetComponent<ParticleSystem>().Play();
        }
        if (n == NoteObject.noteTypes.red)
        {
            buttonHitParticleEffects[1].GetComponent<ParticleSystem>().Play();

        }
        if (n == NoteObject.noteTypes.yellow)
        {
            buttonHitParticleEffects[2].GetComponent<ParticleSystem>().Play();

        }
        if (n == NoteObject.noteTypes.white)
        {
            buttonHitParticleEffects[3].GetComponent<ParticleSystem>().Play();

        }
    }

    public void EndLevel()
    {
        music.Stop();
        levelEnd = true;

        // Start moving the curtains to the right
        Sequence seq = DOTween.Sequence();
        seq.Append(curtain.GetComponent<Transform>().DOMoveX(4, 2f));
        transition = true;
        StartCoroutine(CurtainCall());

        gameScene.SetActive(false);
        startCanvas.SetActive(false);
        endCanvas.SetActive(true);

        performanceText.GetComponent<TMP_Text>().text = Classification((float)notesHit / (float)totalNotes);
        finalScoreText.GetComponent<TMP_Text>().text = "Score: " + currentScore.ToString();
        notesHitText.GetComponent<TMP_Text>().text = "Notes hit: " + notesHit.ToString() + " out of " + totalNotes.ToString();

    }

    private IEnumerator DrawCurtain()
    {
        yield return new WaitForSeconds(1.2f);
        for (int i = 0; i < buttons.Length; i++)
        {
            yield return new WaitForSeconds(.2f);
            buttons[i].GetComponent<ButtonController>().PulseButton();
           
        }
        liveGameCanvas.SetActive(true);
        transition = false;

    }

    private IEnumerator CurtainCall()
    {

        yield return new WaitForSeconds(1.5f);
        GameObject.Find("TopHatParticles").GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2.5f);
        transition = false;

    }

    string Classification(float percentage)
    {
        if (percentage > .9)
        {
            return "Purrfect!";
        } 
        if (percentage > .8)
        {
            return "Clawsome!";
        } 
        if (percentage > .7)
        {
            return "Pawsitively great!";
            
        }
        if (percentage > .6)
        {
            return "Feline fine";
        }

        return "MeOWWWW!";
    }
}
