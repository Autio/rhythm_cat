using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawModel 
{
    public float y;
    public float x;
    public NoteObject.noteTypes pawType;

    public PawModel(float x, float y, NoteObject.noteTypes pawType)
    {
        this.x = x;
        this.y = y;
        this.pawType = pawType;
    }

}
