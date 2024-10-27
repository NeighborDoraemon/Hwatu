using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("")]
[MBTNode("Enemy/BH_Check_Bool")]

public class BH_Check_Bool : Service
{
    [SerializeField] private BoolReference BR_To_Check;

    public override void OnEnter()
    {
        
    }

    public override NodeResult Execute()
    {
        if (BR_To_Check.Value)
        {
            return NodeResult.failure;
        }
        else
        {
            return NodeResult.success;
        }
    }

    public override void Task()
    {
        //throw new System.NotImplementedException();
    }
}
