using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

[AddComponentMenu("")]
[MBTNode("Enemy/BH_Rest")]

public class BH_Rest : Service
{
    private float Rest_Time = 0.0f;
    private float Count_Time = 0.0f;

    [SerializeField] private BoolReference is_At_End;

    public override void OnEnter()
    {
        Rest_Time = Random.Range(1.0f, 3.0f);
        Count_Time = 0.0f;
    }

    public override NodeResult Execute()
    {
        Count_Time += Time.deltaTime;

        if (Count_Time < Rest_Time)
        {
            return NodeResult.running;
        }

        if (is_At_End.Value)
        {
            return NodeResult.failure;
        }

        return NodeResult.success;
    }

    public override void Task()
    {
        //throw new System.NotImplementedException();
    }
}
