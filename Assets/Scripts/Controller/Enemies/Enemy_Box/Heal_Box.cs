using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Box : MonoBehaviour
{

    [SerializeField] private IntReference IR_Attack_Damage;

    public HashSet<Enemy_Basic> healed_Enemy_Basic = new HashSet<Enemy_Basic>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Enemy_Basic Target_Basic = collision.GetComponent<Enemy_Basic>();
            Enemy_Basic Target_Basic_Child = collision.GetComponentInChildren<Enemy_Basic>();

            if (!healed_Enemy_Basic.Contains(Target_Basic_Child)) // 해시셋에 없는 녀석이면 추가
            {
                healed_Enemy_Basic.Add(Target_Basic_Child);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy_Basic Target_Basic = collision.GetComponent<Enemy_Basic>();
            Enemy_Basic Target_Basic_Child = collision.GetComponentInChildren<Enemy_Basic>();

            if (healed_Enemy_Basic.Contains(Target_Basic_Child)) // 해시셋에 있는 녀석이면 삭제
            {
                healed_Enemy_Basic.Remove(Target_Basic_Child);
            }
        }
    }

    public void Heal()
    {
        foreach(Enemy_Basic e_Basic in healed_Enemy_Basic)
        {
            e_Basic.TakeDamage(-IR_Attack_Damage.Value);
            e_Basic.Effect_Healed();
        }
        //Debug.Log(healed_Enemy_Basic.Count);
        //Debug.Log("Heal Complete");
    }
}
