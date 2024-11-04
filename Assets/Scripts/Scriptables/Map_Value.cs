using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewMap_Values", menuName = "Maps/Value")]
public class Map_Value : ScriptableObject
{
    [Header("Maps")]
    public int i_Map_Counter;
    public Vector3 v_Map_Spawnpoint;

    [Header("Camera")]
    public Vector3 v_min_Map_Vector;
    public Vector3 v_Max_Map_Vector;

    [Header("Enemy Spawn Points")]
    public List<Vector3> v_Enemy_Spawn_Points_01;
    public List<Vector3> v_Enemy_Spawn_Points_02;
    public List<Vector3> v_Enemy_Spawn_Points_03;

    [Header("CardBox Spawn Point")]
    public Vector3 v_CardBox_SpawnPoint;

    [Header("About Spawn")]
    public bool Next_To_Kill;

    [Header("Boundary")]
    public Collider2D Collider;

    [HideInInspector]
    public bool Clear_and_Next;
}
