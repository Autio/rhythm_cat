using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public bool debug_mode = false;
    private bool speed_up = false;

    public AudioSource music;
    public GameObject gameScene;
    public GameObject startCanvas;
    public GameObject liveGameCanvas;
    public GameObject endCanvas;
    public GameObject curtain;
    public GameObject[] buttons;
    public GameObject getReady;

    public GameObject[] cats;

    public bool startPlaying;
    public bool levelEnd = false;
    bool ending = false; // Music slows down when ending
    bool startScreen = true;
    bool transition = false;

    public BeatScroller bs;

    public static GameManager instance;

    public int currentScore;
    public int scorePerLongHit = 10; // score boost per increment
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
    public GameObject tryagainText;
    
    public GameObject[] buttonHitParticleEffects;

    public GameObject endCat;

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
        endCat.SetActive(false);


        instance = this;
        maxHealth = health;


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {

            speed_up = !speed_up;

            if (speed_up)
            {
                Time.timeScale *= 5;
                GameObject.Find("LevelSong").GetComponent<AudioSource>().pitch *= 5;
            }
            else
            {

                Time.timeScale = 1;
                GameObject.Find("LevelSong").GetComponent<AudioSource>().pitch = 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {

            GameObject.Find("LevelSong").GetComponent<AudioSource>().pitch *= -1;
            bs.noteDirection *= -1;
        }


        if(ending)
        {
            if(music.pitch > .1f)
            {
                music.pitch -= Time.deltaTime / 3;
            }
        }
        

        if (!transition)
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
                // Show get ready text
                getReady.SetActive(true);
                getReady.GetComponent<GetReady>().BringToView();
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

    public void LongNoteHit()
    {
        currentScore += scorePerLongHit * multiplier;
        scoreText.GetComponent<GenericText>().PulseText();
        scoreText.GetComponent<TMP_Text>().text = currentScore.ToString();

    }

    public void NoteMissed()
    {
        // Immortal if in debug mode
        if (debug_mode == false)
        {
            Debug.Log("Note missed");
            HealthDown();
            // Reset sequence if you miss a note
            sequence = 0;
            notesMissed++;
        }
    }

    void HealthDown()
    {
        health -= 4;
        UpdateHealthBar();
        if (health <= 0)
        {
            // Level lost
            LoseLevel();

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
        string classification = Classification((float)notesHit / (float)totalNotes);

        StartCoroutine(CurtainCall(classification));

        gameScene.SetActive(false);
        startCanvas.SetActive(false);
        endCanvas.SetActive(true);


        // Applause if it wasn't awful
        if (classification != "MeOWWWW!")
        {
            GameObject.Find("SoundEffect").GetComponent<AudioSource>().Play();
        }

        performanceText.GetComponent<TMP_Text>().text = classification;
        finalScoreText.GetComponent<TMP_Text>().text = "Score: " + currentScore.ToString();
        notesHitText.GetComponent<TMP_Text>().text = "Notes hit: " + notesHit.ToString() + " out of " + totalNotes.ToString();

    }

    public void LoseLevel()
    {

        // Slow down the music
        ending = true;

        levelEnd = true;
        StartCoroutine(LevelLoseCoroutine());
        transition = true;



    }

    private IEnumerator DrawCurtain()
    {
        yield return new WaitForSeconds(1.2f);
        // Start putting the lights on
        GameObject.Find("LightingManager").GetComponent<LightingManager>().LightsOn();

        for (int i = 0; i < buttons.Length; i++)
        {
            yield return new WaitForSeconds(.2f);
            buttons[i].GetComponent<ButtonController>().PulseButton();
           
        }
        liveGameCanvas.SetActive(true);
        transition = false;

    }

    private IEnumerator CurtainCall(string classification)
    {

        yield return new WaitForSeconds(1.5f);
        GameObject.Find("TopHatParticles").GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2.5f);
        transition = false;

        // Show happy cat if not clawful
        if (classification != "MeOWWWW!")
        {
            endCat.SetActive(true);
        }

    }

    private IEnumerator LevelLoseCoroutine()
    {


        yield return new WaitForSeconds(2.2f);
            
        music.Stop();

        // Start moving the curtains to the right
        Sequence seq = DOTween.Sequence();
        seq.Append(curtain.GetComponent<Transform>().DOMoveX(4, 2f));

        gameScene.SetActive(false);
        startCanvas.SetActive(false);
        endCanvas.SetActive(true);
        tryagainText.gameObject.SetActive(true);


        string classification = Classification((float)notesHit / (float)totalNotes);


        performanceText.GetComponent<TMP_Text>().text = "Catastrophe!";
        finalScoreText.GetComponent<TMP_Text>().text = "Score: " + currentScore.ToString();
        // Don't show the notes hit when you fail a level
        // notesHitText.GetComponent<TMP_Text>().text = "Notes hit: " + notesHit.ToString() + " out of " + totalNotes.ToString();

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
        if (percentage > .5)
        {
            return "Feline fine";
        }

        return "MeOWWWW!";
    }
    public void SendCatNoteParticle(NoteObject.noteTypes n)
    {
        if (n == NoteObject.noteTypes.blue)
        {
            cats[0].GetComponent<Cat>().PlayNote();
        }
        if (n == NoteObject.noteTypes.red)
        {
            cats[1].GetComponent<Cat>().PlayNote();

        }
        if (n == NoteObject.noteTypes.yellow)
        {
            cats[2].GetComponent<Cat>().PlayNote();

        }
        if (n == NoteObject.noteTypes.white)
        {
            cats[3].GetComponent<Cat>().PlayNote();

        }
    }

}
