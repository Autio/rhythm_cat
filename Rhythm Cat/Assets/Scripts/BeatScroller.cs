using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BeatScroller : MonoBehaviour
{
    // Moves the gridlines and notes on it downwards 
    // Designed to sit on the parent object for notes

    public float beatTempo;         // Should match the tempo of the song in bpm
    public bool hasStarted;
    public int noteDirection = 1;

    public List<Transform> gridBars; // List of all of the horizontal bars

    // Start is called before the first frame update
    void Start()
    {
        // 120 = two units per second
        beatTempo = beatTempo / 60f;    // Converts to seconds 

        
        // Order by Y coordinate
        gridBars = gridBars.OrderBy(t => t.position.y).ToList();

        // Remove unnecessary bars used during level design to save memory during gameplay
        for (int i = gridBars.Count - 1; i >= 42; i--)
        {
            Transform bar = gridBars[i];
            gridBars.Remove(bar);
            Destroy(bar.gameObject);

        }
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
            // Assume an ordered list


            // Old approach
            foreach (Transform bar in gridBars)
            {
                bar.position -= new Vector3(0, beatTempo * noteDirection * Time.deltaTime, 0);

            }

            // We cycle through the existing bars as they go off screen
            // Move the bottom bar up if needed
            Transform bottomBar = gridBars[0];
            if (bottomBar.position.y < -2)
            {
                bottomBar.position = new Vector3(0, gridBars[gridBars.Count - 1].position.y + increment, 0);


                gridBars.Remove(bottomBar);
                gridBars.Add(bottomBar);
            }
            //
            //  if(bar.position.y < -25)
            //    {
            //        // Put a bar back at the top if it's scrolled way down
            //        float maxY = 0;
            //        foreach(Transform bar2 in gridBars)
            //        {
            //            if(bar2.position.y > maxY)
            //            {
            //                maxY = bar2.position.y;
            //            }

            //        }

            //        bar.position = new Vector3(0, maxY + increment, 0);
            //    }
            //}
        }
    }
}
