using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Enemy_Parent, Enemy_Interface
{
    [Header("Delay")]
    [SerializeField] private float f_Delay = 3.0f;


    [SerializeField] private GameObject Obj_HealBox;
    //[SerializeField] private BoolReference BR_Stunned;

    private float f_Attack_Time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!BR_Stunned.Value)
        {
            f_Attack_Time += Time.deltaTime;

            Call_Heal();
        }
    }

    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }

    private void Call_Heal()
    {
        if(f_Attack_Time >= f_Delay)
        {
            Debug.Log("Heal Called");
            Obj_HealBox.GetComponent<Heal_Box>().Heal();
            f_Attack_Time = 0.0f;
        }
    }

    public void Enemy_Stun(float Duration)
    {
        f_Attack_Time = 0.0f;
        Take_Stun(Duration);
    }
}
