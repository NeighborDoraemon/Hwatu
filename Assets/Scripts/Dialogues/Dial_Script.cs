using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Dial_Script
{
    [Tooltip("Character")]
    public string name;

    [Tooltip("Dialogue")]
    public string[] contexts;
}

[System.Serializable]
public class Dialogue_Event
{
    public string name;

    public Vector2 Line;
    public Dial_Script[] Dialogues;
}