using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;
    public bool hasStarted;
    public GameObject horizontalBar;
    public int noteDirection = 1;

    public GameObject[] horizontalBars;

    // Start is called before the first frame update
    void Start()
    {
        // 120 = two units per second
        beatTempo = beatTempo / 60f;

        GameObject startingBar = GameObject.Find("StartingBar");
        // Create a set of horizontal bars
        //for(int i = 0; i < 100; i++)
        //{
        //    GameObject bar = (GameObject)Instantiate(horizontalBar, new Vector3(startingBar.transform.position.x, startingBar.transform.position.y + i * 22.5f, 0), Quaternion.identity);
        //    bar.transform.SetParent(GameObject.Find("HorizontalBarGrid").transform);
        //    //bar.transform.position = new Vector3(bar.transform.parent.localPosition.x - 180, bar.transform.localPosition.y, 0);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasStarted)
        {
/*            if(Input.anyKeyDown)
            {
                hasStarted = true;
            }*/
        }
        else
        {

            transform.position -= new Vector3(0, beatTempo * noteDirection * Time.deltaTime, 0);

            float increment = .5f;
            // Also scroll the horizontal bars
            foreach(GameObject bar in horizontalBars)
            {
                bar.transform.position -= new Vector3(0, beatTempo * noteDirection * Time.deltaTime, 0);
                if(bar.transform.position.y < -75)
                {
                    // Put the bar at the top
                    float maxY = 0;
                    foreach(GameObject bar2 in horizontalBars)
                    {
                        if(bar2.transform.position.y > maxY)
                        {
                            maxY = bar2.transform.position.y;
                        }

                    }

                    //Debug.Log("MaxY " + maxY);
                    bar.transform.position = new Vector3(0, maxY + increment, 0);
                }
            }
        }
    }
}
