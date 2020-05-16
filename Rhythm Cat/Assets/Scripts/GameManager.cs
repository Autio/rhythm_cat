using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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

    public int health = 40;
    private int maxHealth;

    public GameObject scoreText;
    public GameObject multiplierText;
    public GameObject sequenceText;
    public GameObject healthBar;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        maxHealth = health;
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
        HealthUp();

        currentScore += scorePerNote * multiplier;
        try
        {
            scoreText.GetComponent<TMP_Text>().text = currentScore.ToString();
            sequence++;
            sequenceText.GetComponent<TMP_Text>().text = sequence.ToString();


            // Check if it is time to bump up the multiplier
            if (CheckMultiplier())
            {
                multiplier++;
                multiplierText.GetComponent<TMP_Text>().text = multiplier.ToString() +"X";
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
        HealthDown();
        // Reset sequence if you miss a note
        sequence = 0;

    }

    void HealthDown()
    {
        health -= 4;
        UpdateHealthBar();
        if (health <= 0)
        {
            // Level lost
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

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
        if(sequence % multiplierThreshold == 0)
        {
            return true;
        }
        return false;
    }

    void UpdateHealthBar()
    {

        healthBar.GetComponent<Slider>().value = (float)health/ (float)maxHealth;
    }

}
