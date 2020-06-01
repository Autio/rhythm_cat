using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    // Moves the gridlines and notes on it downwards 
    // Designed to sit on the parent object for notes

    public float beatTempo;         // Should match the tempo of the song in bpm
    public bool hasStarted;
    public int noteDirection = 1;

    public GameObject[] horizontalBars; // Stores all the horizontal staves to be moved down

    // Start is called before the first frame update
    void Start()
    {
        // 120 = two units per second
        beatTempo = beatTempo / 60f;    // Converts to seconds 
        GameObject startingBar = GameObject.Find("StartingBar");

    }

    // Update is called once per frame
    void Update()
    {
        if(!hasStarted)
        {
            // hasStarted gets set by GameManager
        }
        else
        {

            // Moves the notes
            transform.position -= new Vector3(0, beatTempo * noteDirection * Time.deltaTime, 0);

            
            float increment = .5f;
            // Also scroll the horizontal bars
            foreach(GameObject bar in horizontalBars)
            {
                bar.transform.position -= new Vector3(0, beatTempo * noteDirection * Time.deltaTime, 0);
                if(bar.transform.position.y < -25)
                {
                    // Put a bar back at the top if it's scrolled way down
                    float maxY = 0;
                    foreach(GameObject bar2 in horizontalBars)
                    {
                        if(bar2.transform.position.y > maxY)
                        {
                            maxY = bar2.transform.position.y;
                        }

                    }

                    bar.transform.position = new Vector3(0, maxY + increment, 0);
                }
            }
        }
    }
}
