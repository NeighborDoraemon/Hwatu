using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] private TextMeshPro Damage_Text;

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

    [HideInInspector] public bool For_Boss_Once = true;

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
        else
        {
            if (IR_Health.Value > 0)
            {
                Damage_Effect();
                ShowDamage(damage);
            }
        }


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
            if (Second_Phase_Script != null && For_Boss_Once)
            {
                Second_Phase_Controller = Second_Phase_Script as Enemy_Second_Phase;
                Second_Phase_Controller.Call_Second_Phase();
                For_Boss_Once = false;
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
        if (colorCoroutine != null)
        {
            StopCoroutine(colorCoroutine);
        }

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

    public void ShowDamage(int Damage)
    {
        if (Damage_Text != null)
        {
            //if(this.gameObject.transform.rotation.y >= 90)
            //{
            //    Damage_Text.gameObject.GetComponent<RectTransform>().rotation = new Quaternion(0.0f, 180.0f, 0.0f, 0.0f);
            //}
            //else
            //{
            //    Damage_Text.gameObject.GetComponent<RectTransform>().rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            //}
                Damage_Text.text = Damage.ToString();
            Color color = Damage_Text.color;

            StartCoroutine(Fade_Sprite(Damage_Text, 1.0f, 0.5f, 0.0f));
            StartCoroutine(Fade_Sprite(Damage_Text, 0.0f, 0.5f, 0.5f));
        }
    }

    //private IEnumerator FadeOut()
    //{
    //    while (Text.color.a > 0.0f)
    //    {
    //        Text.color.a -= Time.deltaTime * 0.5f;
    //        yield return null;
    //    }
    //}

    private IEnumerator Fade_Sprite(TextMeshPro sprite, float targetAlpha, float duration, float delay)
    {
        Vector3 vec3 = Damage_Text.gameObject.GetComponent<RectTransform>().localScale;
        yield return new WaitForSeconds(delay);

        Color color = sprite.color;
        float startAlpha = color.a; // 현재 알파값을 시작점으로 저장

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            if (this.gameObject.transform.rotation.y > 0)
            {
                if (vec3.x > 0)
                {
                    vec3.x = vec3.x * -1.0f;
                }
                Damage_Text.gameObject.GetComponent<RectTransform>().localScale = vec3;
            }
            else
            {
                if (vec3.x < 0)
                {
                    vec3.x = vec3.x * -1.0f;
                }
                Damage_Text.gameObject.GetComponent<RectTransform>().localScale = vec3;
            }

            float progress = t / duration;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            color.a = alpha;
            sprite.color = color;

            //float alpha = Mathf.Lerp(0.0f, targetAlpha, t / duration);
            //color.a = alpha;
            //sprite.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        sprite.color = color;

        //for (float t = 0.0f; t < duration; t += Time.deltaTime)
        //{
        //    float alpha = Mathf.Lerp(targetAlpha, 0.0f, t / duration);
        //    color.a = alpha;
        //    sprite.color = color;

        //    yield return null;
        //}

        //color.a = 0.0f;
        //sprite.color = color;
    }

    public void Set_Once()
    {
        For_Boss_Once = true;
    }
}
