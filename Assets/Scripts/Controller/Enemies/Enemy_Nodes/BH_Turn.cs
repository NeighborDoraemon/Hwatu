using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

[AddComponentMenu("")]
[MBTNode("Enemy/BH_Turn")]


public class BH_Turn : Service
{
    [Header("Bools")]
    [SerializeField] private BoolReference is_Facing_Left;
    [SerializeField] private BoolReference is_At_End;
    [SerializeField] private BoolReference is_Chasing;

    [Header("Others")]
    [SerializeField] private GameObject Obj_Enemy;

    public override void OnEnter()
    {
        if (Obj_Enemy.gameObject.transform.rotation.y != 0.0f)
        {
            is_Facing_Left.Value = false;
            Obj_Enemy.gameObject.transform.rotation = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);
        }
        else
        {
            is_Facing_Left.Value = true;
            Obj_Enemy.gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    public override NodeResult Execute()
    {
        if (is_Chasing.Value)
        {
            return NodeResult.failure;
        }

        Quaternion quater = Obj_Enemy.gameObject.transform.rotation;

        if (is_Facing_Left.Value) // ÁÂÃø º¸´ÂÁß
        {
            //Debug.Log("Enemy Turned");
            is_Facing_Left.Value = false;
            quater.y = 180.0f;

            Obj_Enemy.gameObject.transform.rotation = quater;
            is_At_End.Value = false;
        }
        else if (!is_Facing_Left.Value)
        {
            //Debug.Log("Enemy Turned");
            is_Facing_Left.Value = true;
            //Obj_Enemy.gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            quater.y = 0.0f;

            Obj_Enemy.gameObject.transform.rotation = quater;
            is_At_End.Value = false;
        }
        return NodeResult.success;
    }





    public override void Task()
    {
        //throw new System.NotImplementedException();
    }
}
