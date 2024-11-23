using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName = "NewMap_Values", menuName = "Maps/Value")]
public class Map_Value : ScriptableObject
{
    [Header("Maps")]
    public int i_Map_Counter;
    public Vector3 v_Map_Spawnpoint;


    [Header("New Enemy Spawn Points")]
    public List<Vectors> v_New_Spawn_Points;

    [Header("New Enemy Spawn Data")]
    public List<e_Numbers> i_Enemy_Index;

    [Header("New Enemy Wave Count")]
    public int i_How_Many_Wave;

    [Header("CardBox Spawn Point")]
    public Vector3 v_CardBox_SpawnPoint;

    [Header("Boundary")]
    public Collider2D Collider;
}

[System.Serializable]
public class Vectors
{
    public List<Vector3> v_Dataes;
}

[System.Serializable]
public class e_Numbers
{
    public List<int> i_enemy_Index;
}