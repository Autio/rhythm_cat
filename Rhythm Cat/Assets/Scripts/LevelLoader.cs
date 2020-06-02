using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;

    public List<PawModel> levelPaws;
    // Read the notes of the level into memory
    // Then assign them in place by moving the ones the player has moved past
    // To their right position
    GameManager gm;
    [SerializeField]
    private GameObject[] notePrefabs;
    Transform noteParent;
    float noteOffsetY; // What is the starting position of the note-holding gameobject

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;    // Ensures there's only one level loader only

        gm = GameManager.instance;

        levelPaws = new List<PawModel>();

        noteParent = GameObject.Find("NOTES").transform;
    }
    void Start()
    {
        noteOffsetY = noteParent.position.y;
    }
    public void LoadNotes()
    {
        foreach (GameObject note in GameManager.instance.notes)
        {
            NoteObject.noteTypes noteType = note.GetComponent<NoteObject>().thisNoteType;
            PawModel newPaw = new PawModel(note.transform.position.x, note.transform.position.y, noteType);
            levelPaws.Add(newPaw);
        }

        // Order by Y coordinate
        levelPaws.OrderBy(w => w.y).ToList();

        // Arbitrarily delete the objects after the first x
        foreach (GameObject note in GameManager.instance.notes)
        {
            for (int i = 19; i < levelPaws.Count; i++)
            {
                if (note.transform.position.x == levelPaws[i].x && note.transform.position.y == levelPaws[i].y)
                {
                    Destroy(note);
                }
            }
        }
        Debug.Log(levelPaws);
        Debug.Log(GameManager.instance.notes);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextNote()
    {
        Debug.Log("Next note being created");
        // Whenever a note goes out of view on the track
        // Generate the next note for the track so there's always only up to x amount
        if(levelPaws.Count > 0)
        {
            // Assume the list is ordered by ascending Y
            PawModel paw = levelPaws[0];
            int pawTypeIndex = (int)paw.pawType;
            Debug.Log(pawTypeIndex);
            GameObject newNote = Instantiate(notePrefabs[pawTypeIndex], new Vector2(paw.x, noteParent.position.y + paw.y - noteOffsetY), Quaternion.identity);
            newNote.transform.parent = noteParent;

            // Remove that entry from the list
            levelPaws.RemoveAt(0);
        }
    }
}
