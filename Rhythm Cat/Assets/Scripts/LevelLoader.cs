using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelLoader : MonoBehaviour
{
    public List<PawModel> levelPaws;
    // Read the notes of the level into memory
    // Then assign them in place by moving the ones the player has moved past
    // To their right position
    GameManager gm;
    

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        levelPaws = new List<PawModel>();
    }

    public void LoadNotes()
    {
        foreach (GameObject note in gm.notes)
        {
            levelPaws.Add(new PawModel(note.transform.position.x, note.transform.position.y, note.GetComponent<NoteObject>().thisNoteType));
        }

        // Order by Y coordinate
        levelPaws.OrderBy(w => w.y).ToList();

        // Arbitrarily delete the objects after the first x
        foreach (GameObject note in gm.notes)
        {
            for (int i = 29; i < levelPaws.Count; i++)
            {
                if (note.transform.position.x == levelPaws[i].x && note.transform.position.y == levelPaws[i].y)
                {
                    Destroy(note);
                }
            }
        }
        Debug.Log(levelPaws);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
