using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Enemy_Basic : MonoBehaviour, Enemy_Interface
{
    [Header("BB")]
    [SerializeField] public IntReference IR_Health;
    //[SerializeField] private BoolReference BR_Stunned;

    [Header("Bool")]
    [SerializeField] private bool is_Boos_Object = false;

    [SerializeField] private Obj_ScareCrow scarecrow = null;

    [Header("Money")]
    [SerializeField] private int min_Money_Drop;
    [SerializeField] private int Max_Money_Drop;

    [Header("Effects")]
    [SerializeField] private Animator Effect_Animator;
    [SerializeField] private SpriteRenderer Sp_Renderer;
    private Coroutine colorCoroutine;

    [Header("Others")]
    [SerializeField] private MonoBehaviour Second_Phase_Script;
    [SerializeField] private bool is_No_Counted = false; // 적이 죽어도 카운트되지 않음

    private Enemy_Second_Phase Second_Phase_Controller;

    private PlayerCharacter_Controller player_Con;

    private GameObject Target_Player;
    private int i_Max_Health;

    private bool is_Immortal = false; // 적이 무적 상태인지 확인

    private Coroutine bleedingCoroutine;
    private Enemy_Generator Ene_Generator;

    private bool is_Dead = false;

    [HideInInspector]
    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        player_Con = player;
    }

    public void Get_Enemy_Generator(Enemy_Generator enemy_Generator)
    {
        Ene_Generator = enemy_Generator;
    }

    public void Start()
    {
        i_Max_Health = IR_Health.Value;
    }

    public void Change_Immortal()
    {
        is_Immortal = !is_Immortal;
    }

    public void TakeDamage(int damage)
    {
        if(damage < 0)
        {
            Debug.Log("Get Heal");
        }
        //else
        //{
        //    Damage_Effect();
        //}


        if (is_Immortal)
        {
            Debug.Log("Enemy is Immortal, no damage taken.");
            return;
        }

        IR_Health.Value -= damage;
        Debug.Log($"Enmey took {damage} damage. Remaining health : {IR_Health.Value}");

        if(IR_Health.Value > i_Max_Health)
        {
            IR_Health.Value = i_Max_Health;
        }

        if (IR_Health.Value <= 0 && !is_Boos_Object)
        {
            Die();
        }
        else if(IR_Health.Value <= 0 && is_Boos_Object)
        {
            if (Second_Phase_Script != null)
            {
                Second_Phase_Controller = Second_Phase_Script as Enemy_Second_Phase;
                Second_Phase_Controller.Call_Second_Phase();
            }
        }

        if (scarecrow != null)
        {
            scarecrow.ShowDamage(damage);
        }
    }

    public void Call_Custom_Die()
    {
        Die();
    }

    private void Die() 
    {
        if (is_Dead) return; // 이미 죽은 상태라면 중복 실행 방지
        //if (colorCoroutine != null)
        //{
        //    StopCoroutine(colorCoroutine);
        //}

        if (bleedingCoroutine != null)
        {
            StopCoroutine(bleedingCoroutine);
            bleedingCoroutine = null;
        }

        if (!is_No_Counted)
        {
            Ene_Generator.Enemy_Died();


            if (player_Con == null)
            {
                Debug.Log("Player null");
            }

            int DropMoney = Random.Range(min_Money_Drop, Max_Money_Drop);
            player_Con.Add_Player_Money(DropMoney);

            player_Con.Enemy_Killed();
        }

        if (this.transform.parent != null)
        {
            Destroy(this.transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        is_Dead = true;
    }

    public void Bleeding_Attack(int Tick_Damage, int Count, float Delay)
    {
        if (bleedingCoroutine != null)
        {
            StopCoroutine(bleedingCoroutine);
        }
        bleedingCoroutine = StartCoroutine(Bleeding_Coroutine(Tick_Damage, Count, Delay));
    }

    private IEnumerator Bleeding_Coroutine(int Tick_Damage, int Count, float Delay)
    {
        for (int i = 0; i < Count; i++)
        {
            yield return new WaitForSeconds(Delay);
            TakeDamage(Tick_Damage);
        }
    }

    public void Effect_Healed()
    {
        Effect_Animator.SetTrigger("Trigger_Healed");
    }

    private void Damage_Effect()
    {
        if (Sp_Renderer != null)
        {
            Sp_Renderer.color = Color.red;

            if (colorCoroutine != null)
            {
                StopCoroutine(colorCoroutine);
            }
            colorCoroutine = StartCoroutine(Recover_Color());
        }
    }

    private IEnumerator Recover_Color()
    {
        // Gradually change the color back to white over time
        float duration = 0.5f; // Duration of the color change
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Sp_Renderer.color = Color.Lerp(Color.red, Color.white, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Sp_Renderer.color = Color.white; // Ensure the color is set to white at the end
    }

    public Enemy_Generator Give_Enemy_Generator()
    {
               return Ene_Generator;
    }
}
