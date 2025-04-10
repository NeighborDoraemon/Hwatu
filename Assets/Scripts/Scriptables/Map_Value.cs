using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName = "NewMap_Values", menuName = "Maps/Value")]
public class Map_Value : ScriptableObject
{
    [Header("Maps")]
    public Vector3 v_Map_Spawnpoint;


    [Header("New Enemy Spawn Points")]
    public List<Vectors> v_New_Spawn_Points = new List<Vectors>();

    [Header("New Enemy Spawn Data")]
    public List<e_Numbers> i_Enemy_Index = new List<e_Numbers>();

    [Header("New Enemy Wave Count")]
    public int i_How_Many_Wave;

    [Header("CardBox Spawn Point")]
    public Vector3 v_CardBox_SpawnPoint;

    [Header("Boundary")]
    public Collider2D Collider;

    [Header("Minimap")]
    public Vector3 v_Minimap_Point;
    public float f_Minimap_Size;
}

[System.Serializable]
public class Vectors
{
    public List<Vector3> v_Dataes = new List<Vector3>();
}

[System.Serializable]
public class e_Numbers
{
    public List<int> i_enemy_Index = new List<int>();
}