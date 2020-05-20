using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class GridCreator 
{
    public GameObject horizontalBar;
    GameObject startingBar;

    [MenuItem("GameObject/3D Object/Create Gridlines")]
    // Start is called before the first frame update
    static void Create()
    {
        for(int i = 0; i < 300; i++)
        {
            Debug.Log("Whatnow");
            GameObject newBar = (GameObject)PrefabUtility.InstantiatePrefab(Selection.activeObject as GameObject);
            GameObject start = GameObject.Find("StartingBar");

            newBar.GetComponent<RectTransform>().position = new Vector3(start.transform.GetComponent<RectTransform>().position.x, start.transform.GetComponent<RectTransform>().position.y + i * .5f, 0);
            Color c = newBar.GetComponent<Image>().color;
            if ( i%6 == 0 )
            {
                newBar.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 13f/255f);
            } else
            {
                newBar.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 3f / 255f);

            }

            //bar.transform.SetParent(GameObject.Find("HorizontalBarGrid").transform);
            //bar.transform.position = new Vector3(bar.transform.parent.localPosition.x - 180, bar.transform.localPosition.y, 0);
        }
    }

}
