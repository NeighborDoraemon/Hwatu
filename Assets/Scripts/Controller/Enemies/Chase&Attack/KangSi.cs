using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KangSi : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Values")]
    [SerializeField] private float Jump_power = 5.0f;
    [SerializeField] private float Side_power = 3.0f;


    private float Attack_Delay = 0.5f;
    private float Attack_Timer = 0.0f;

    [Header("Others")]
    [SerializeField] private Crash_Box Enemy_CB;
    //[SerializeField] private Animator KangSi_Animator;

    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        Attack_Timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Attack_Timer += Time.deltaTime;
        if (Attack_Timer >= Attack_Delay)
        {
            KangSi_Jump();
            Attack_Timer = 0.0f;
        }
    }

    private void KangSi_Jump()
    {
        Enemy_CB.Damage_Once = true;

        if(BR_Facing_Left.Value)
        {
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-Side_power, Jump_power), ForceMode2D.Impulse);
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Side_power, Jump_power), ForceMode2D.Impulse);
        }
    }

    public void Enemy_Stun(float Duration)
    {
        Take_Stun(Duration);
    }
}
