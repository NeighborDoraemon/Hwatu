using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;


[AddComponentMenu("")]
[MBTNode("Enemy/BH_Patrol")]

public class BH_Patrol : Service
{
    [Header("Nums")]
    [SerializeField] private FloatReference e_Move_Speed;
    [SerializeField] private FloatReference e_Moving_Time;
    [SerializeField] private FloatReference e_Moving_Random;



    [Header("Bools")]
    [SerializeField] private BoolReference is_Facing_Left;
    [SerializeField] private BoolReference is_Chasing;
    [SerializeField] private BoolReference is_At_End;

    [Header("Others")]
    [SerializeField] private GameObject Obj_Enemy;
    [SerializeField] private Animator Enemy_Animator;

    private void Reset_Random()
    {
        e_Moving_Random.Value = Random.Range(2.0f, 5.0f);
        e_Moving_Time.Value = 0.0f;
    }
    public override NodeResult Execute()
    {
        e_Moving_Time.Value += Time.deltaTime;

        if (is_Chasing.Value) // ���������� �÷��̾ ���Դ�
        {
            Enemy_Animator.SetBool("is_Moving", false);
            return NodeResult.failure;
        }

        if (is_At_End.Value) // ������ ���� ���ִ�
        {
            Enemy_Animator.SetBool("is_Moving", false);
            return NodeResult.failure;
        }

        if (e_Moving_Time.Value < e_Moving_Random.Value) // ���� �̵� ��
        {
            //Enemy_Animator.SetBool("is_Moving", true);
            Wandering();
        }
        else if (e_Moving_Time.Value >= e_Moving_Random.Value) // �̵� �ð� ����
        {
            Reset_Random();
            Enemy_Animator.SetBool("is_Moving", false);
            return NodeResult.success;
        }
        Enemy_Animator.SetBool("is_Moving", true);
        return NodeResult.running;
    }

    private void Wandering()
    {
        if (is_Facing_Left.Value)
        {
            Obj_Enemy.transform.Translate(Vector3.left * e_Move_Speed.Value * Time.deltaTime);
            //enemy_Rigid.velocity = new Vector3(-e_Move_Speed.Value, 0.0f, 0.0f);
            //Debug.Log("Now Moving");
        }
        else if (!is_Facing_Left.Value)
        {
            Obj_Enemy.transform.Translate(Vector3.right * -e_Move_Speed.Value * Time.deltaTime);
            //enemy_Rigid.velocity = new Vector2(e_Move_Speed.Value, 0.0f);
            //Debug.Log("Now Moving");
        }
    }

    public override void Task()
    {
        //throw new System.NotImplementedException();
    }
}
