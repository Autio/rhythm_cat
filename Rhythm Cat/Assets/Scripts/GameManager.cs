using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// This is the main game manager class where the sequence of the game, the main game loop, scoring, etc are set
    /// </summary>
    
    // Makes sure there's only one GameManager
    public static GameManager instance;

    // This makes the player invulnerable
    public bool debug_mode = false;

    // Toggle for faster play mode 
    private bool speed_up = false;
    private float[] gameSpeeds = { 1, 1.5f, 2f };
    private int currentGameSpeedIndex = 0;

    public AudioSource music;           // Music source object
    public GameObject gameScene;        // The game scene holder for easy on/off
    public GameObject startCanvas;      // What gets shown as the starting screen
    public GameObject liveGameCanvas;   // What gets shown during gameplay
    public GameObject endCanvas;        // What gets shown as the ending screen
    public GameObject curtain;          // Curtain that gets drawn at the start and closed at the end
    public GameObject[] buttons;        // The four buttons players have to press
    public GameObject[] cats;           // The cats on the right side of the main playing area

    /// <summary>
    /// GAME SEQUENCING
    /// </summary>
    public bool startPlaying;
    public bool levelEnd = false;
    bool ending = false;            // Music slows down when ending
    bool startScreen = true;        
    bool transition = false;        // Allows disabling of player input during transitions

    // Beat scroller handles the motion of the music grid and notes on it
    public BeatScroller bs;

    public static float buttonY;    // Y position of the buttons

    // Health and health-bar
    public int health = 40;         // Hit points at any given time
    private int maxHealth;          // Set to health at the start and used to compare how full the health bar should be

    public GameObject healthBar;
    public Color goodHealthColor;   // What colour is the bar when things are good
    public Color badHealthColor;    // What colour is the bar when things are bad
    public GameObject fill;         // The health bar fill
    public GameObject handle;       // The health bar icon / handle 

    /// <summary>
    /// SCORING
    /// </summary>
    public int currentScore;
    public int scorePerLongHit = 10;    // score boost per increment
    public int scorePerNormalHit = 80;
    public int scorePerGoodHit = 100;
    public int scorePerPerfectHit = 120;
    public int sequence = 0;            // Sequence of correctly hit notes

    public int multiplier = 1;          // Current multiplier
    // How many hits do you need to get in sequence before the multiplier is bumped up
    public int[] multiplierThresholds = { 4, 8, 16, 32, 64, 128, 256, 512 };

    int notesHit;
    int notesMissed;
    int totalNotes; // How many notes on the scene

    int goodHits;
    int perfectHits;

    // AUDIO
    public AudioClip[] songsBySpeed;

    /// <summary>
    /// Text objects
    /// </summary>
    public GameObject getReady;

    public GameObject speedText;
    public GameObject speedTextLabel;

    // Texts during gameplay
    public GameObject scoreText;
    public GameObject multiplierText;
    public GameObject sequenceText;

    // Texts for the end canvas
    public GameObject performanceText;
    public GameObject finalScoreText;
    public GameObject notesHitText;
    public GameObject tryagainText;
    public GameObject goodHitsText;
    public GameObject perfectHitsText;
    
    // Cat to show at the end after a succesfull level
    public GameObject endCat;

    public GameObject[] buttonHitParticleEffects;
    public GameObject[] notes;
    public GameObject[] longNotes;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;    // Ensures there's only one game manager only
        maxHealth = health;

        buttonY = buttons[0].transform.position.y;

        // Set basic speed and pitch
        Time.timeScale = gameSpeeds[0];
        GameObject.Find("LevelSong").GetComponent<AudioSource>().pitch = gameSpeeds[0];

        notes = GameObject.FindGameObjectsWithTag("Note");
        longNotes = GameObject.FindGameObjectsWithTag("LongNote");
        totalNotes = notes.Length + longNotes.Length; // Count all notes
        Debug.Log("Total notes in this level: " + totalNotes.ToString());

        // Make sure that only the canvases and views we want to be active at the start
        // Actually are active
        gameScene.SetActive(false);
        startCanvas.SetActive(true);
        liveGameCanvas.SetActive(false);
        endCanvas.SetActive(false);
        endCat.SetActive(false);
        
    }

    // TODO: Level switching
    // TODO: Level editor

    /// <summary>
    /// MAIN GAME LOOP
    /// </summary>
    /// 
    public void CycleSpeed()
    {

        // Toggle
        currentGameSpeedIndex++;
 
        if (currentGameSpeedIndex > (gameSpeeds.Length - 1))
        {
            currentGameSpeedIndex = 0;
        }

        Debug.Log("Game speed set to " + gameSpeeds[currentGameSpeedIndex].ToString());

        Time.timeScale = gameSpeeds[currentGameSpeedIndex];
        GameObject.Find("LevelSong").GetComponent<AudioSource>().clip = songsBySpeed[currentGameSpeedIndex];

        // DOESN'T WORK IN WEBGL, SO WE'LL JUST USE DIFFERENT AUDIO FILES
        // Change only tempo but try to maintain 
//        GameObject.Find("LevelSong").GetComponent<AudioSource>().pitch = gameSpeeds[currentGameSpeedIndex];
        // Set the appropriate mixer snapshot so that 
 //       snapshots[currentGameSpeedIndex].TransitionTo(.1f);

        try
        {
            speedText.GetComponent<TMP_Text>().text = "< " + gameSpeeds[currentGameSpeedIndex].ToString() + "x >";
        }
        catch
        {

        }

    }

    public void ExitStartScreen()
    {
        // enable main game and hide the start screen
        // Start moving the curtains to the right 
        startScreen = false;
        Sequence seq = DOTween.Sequence();
        seq.Append(curtain.GetComponent<Transform>().DOMoveX(-12.64f, 2f));
        transition = true;
        StartCoroutine(DrawCurtain());
        startCanvas.SetActive(false);
        gameScene.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        
        // Debug key which makes the game run faster
        //if(Input.GetKeyDown(KeyCode.F1))
        //{
        //    CycleSpeed();
        //}

        // Reverses the song direction 
        if (Input.GetKeyDown(KeyCode.F2))
        {

            GameObject.Find("LevelSong").GetComponent<AudioSource>().pitch *= -1; // Song reversal
            bs.noteDirection *= -1; // Movement of grid and notes on the grid
        }

        // If the player is losing the level, make the music slow down to a halt gradually
        if(ending)
        {
            if(music.pitch > .02f)
            {
                music.pitch -= Time.deltaTime / 3;
            }
        }
        
        if (!transition)
        {

            if(startScreen)
            {
                if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    CycleSpeed();
                }

                // Any key take the player from the start screen
                if (Input.GetKey(KeyCode.Space))
                {
                    ExitStartScreen();
                }
            }
            else if (!startPlaying)
            {
                // Actually start the music rolling and the game playing once the curtain is drawn
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
                // Restart the game, but only if the level has ended
                if(Input.anyKeyDown)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }

    // What happens when any note has been successfully hit
    public void NoteHit()
    {
        HealthUp();
        notesHit++; // We increase our tally of hit notes

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

    /// <summary>
    /// Pathways for the different types of note hits: Normal, Good, Perfect, Long
    /// </summary>

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
        goodHits++;

    }

    public void PerfectHit()
    {
        currentScore += scorePerPerfectHit * multiplier;
        Debug.Log("Perfect hit!");
        NoteHit();
        perfectHits++;

    }

    public void LongNoteHit()
    {
        currentScore += scorePerLongHit * multiplier;
        scoreText.GetComponent<GenericText>().PulseText();
        scoreText.GetComponent<TMP_Text>().text = currentScore.ToString();

    }

    // What happens when you miss a note
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

    // Returns true if the current sequence is longer than the currently valid threshold
    bool CheckMultiplier()
    {
        if (sequence > multiplierThresholds[multiplier - 1])
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// HEALTH MANAGEMENT
    /// </summary>
    void HealthDown()
    {
        health -= 4; // FIXME: hard-coded health decrement
        UpdateHealthBar();

        // Check for the end of the game due to health dropping to 0 or below
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
        health += 2; // FIXME: hard-coded health increment
        health = Mathf.Clamp(health, 1, maxHealth);
        UpdateHealthBar();
        
    }

    void UpdateHealthBar()
    {
        float ratio = (float)health / (float)maxHealth;
        healthBar.GetComponent<Slider>().value = ratio;

        // Check if the health bar color should change
        if(ratio < .3f) 
        {
            fill.GetComponent<Image>().color = badHealthColor;
            handle.GetComponent<Image>().color = badHealthColor;
        } else
        {
            fill.GetComponent<Image>().color = goodHealthColor;
            handle.GetComponent<Image>().color = goodHealthColor;
        }
    }

    /// <summary>
    /// LEVEL ENDINGS
    /// </summary>
    
    // When the player gets to the end of the track
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

        // Hide the visible scene and activate the end screen
        // FIXME: Smarter way to handle activations and deactivations?
        gameScene.SetActive(false);
        startCanvas.SetActive(false);
        endCanvas.SetActive(true);
        tryagainText.gameObject.SetActive(false);

        // Play the applause sound effect if it wasn't awful
        if (classification != "MeOWWWW!")
        {
            GameObject.Find("SoundEffect").GetComponent<AudioSource>().Play();
        }
        else
        {
            // TODO: Play another effect if the song WAS awful
        }
        
        // Update the texts on the end screen
        performanceText.GetComponent<TMP_Text>().text = classification;
        finalScoreText.GetComponent<TMP_Text>().text = "Score: " + currentScore.ToString();
        notesHitText.GetComponent<TMP_Text>().text = "Notes hit: " + notesHit.ToString() + " out of " + totalNotes.ToString();
        goodHitsText.GetComponent<TMP_Text>().text = "Good hits: " + goodHits.ToString();
        perfectHitsText.GetComponent<TMP_Text>().text = "Purrfect hits: " + perfectHits.ToString();


    }

    // What happens if the player runs out of health
    public void LoseLevel()
    {

        ending = true;
        levelEnd = true;
        StartCoroutine(LevelLoseCoroutine());
        transition = true;

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
        notesHitText.GetComponent<TMP_Text>().text = "";
        goodHitsText.GetComponent<TMP_Text>().text = "Good hits: " + goodHits.ToString();
        perfectHitsText.GetComponent<TMP_Text>().text = "Purrfect hits: " + perfectHits.ToString();

        yield return new WaitForSeconds(2.5f);
        transition = false;

    }

    // Coroutine to open the curtain
    private IEnumerator DrawCurtain()
    {
        yield return new WaitForSeconds(1.2f);

        // Start putting the lights on
        GameObject.Find("LightingManager").GetComponent<LightingManager>().LightsOn();

        // Pulse the player buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            yield return new WaitForSeconds(.2f);
            buttons[i].GetComponent<ButtonController>().PulseButton();
           
        }

        // After all of the above is done, activate the live game canvas
        liveGameCanvas.SetActive(true);
        // Finish the transition, allow players to interact again
        transition = false;

    }


    // Coroutine to close the curtain at the end of a level
    private IEnumerator CurtainCall(string classification)
    {

        yield return new WaitForSeconds(1.5f);

        // Show happy cat and throw top hats if not clawful
        if (classification != "MeOWWWW!")
        {
            endCat.SetActive(true);
            GameObject.Find("TopHatParticles").GetComponent<ParticleSystem>().Play();

        }

        yield return new WaitForSeconds(2.5f);
        transition = false;

    }


    // Classifying the player's achievement on the basis of the proportion of notes hit
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

    // Get the cats to send a musical note from their mouths
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

    // Get the buttons at the bottom to emit a colourful effect when you hit a note
    public void ActivateNoteHitParticles(NoteObject.noteTypes n)
    {
        if (n == NoteObject.noteTypes.blue)
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

}
