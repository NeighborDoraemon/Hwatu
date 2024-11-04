using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_Effect_Data", menuName = "ScriptableObjects/Weapon_Effect_Data")]
public class Weapon_Effect_Data : ScriptableObject
{
    [System.Serializable]
    public class Frame_Effect_Info
    {
        public int frame_Number;
        public Sprite effect_Sprites;
        public Vector3 position_Offset;
        public float duration;
    }

    [System.Serializable]
    public class Effect_Info
    {
        public string motion_Name;
        public List<Frame_Effect_Info> frame_Effects;
    }

    public List<Effect_Info> effects;

    public Effect_Info Get_Effect_Info(string motion_Name)
    {
        return effects.Find(effect => effect.motion_Name == motion_Name);
    }
}
