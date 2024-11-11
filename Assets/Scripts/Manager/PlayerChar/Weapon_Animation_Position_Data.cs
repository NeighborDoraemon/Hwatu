using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_Animation_Position_Data", menuName = "ScriptableObjects/Weapon_Animation_Position_Data")]
public class Weapon_Animation_Position_Data : ScriptableObject
{
    public string animation_Name;

    public List<Vector3> frame_Pos = new List<Vector3>();
    public List<Vector3> frame_Rotations = new List<Vector3>();
    public List<Vector3> frame_Scales = new List<Vector3>();

    public Vector3 Get_Position(int frame)
    {
        if (frame >= 0 && frame < frame_Pos.Count)
            return frame_Pos[frame];
        return Vector3.zero;
    }

    public Quaternion Get_Rotation(int frame) 
    {
        if (frame >= 0 && frame < frame_Rotations.Count)
            return Quaternion.Euler(frame_Rotations[frame]);
        return Quaternion.identity;
    }

    public Vector3 Get_Scale(int frame)
    {
        if (frame >= 0 && frame < frame_Scales.Count)
            return frame_Scales[frame];
        return Vector3.one;
    }
}
