using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Trap_Projectile : MonoBehaviour
{
    public int Arrow_Damage = 5;

    private void Start()
    {
        Time_Destroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(Arrow_Damage);

            if (collision != null)
            {
                if (this.transform.position.x < collision.gameObject.transform.position.x) // 플레이어의 좌측에 맞음
                {
                    collision.gameObject.GetComponentInParent<PlayerCharacter_Controller>().Weak_Knock_Back(1, 0.2f, 1.5f);
                }
                else
                {
                    collision.gameObject.GetComponentInParent<PlayerCharacter_Controller>().Weak_Knock_Back(-1, 0.2f, 1.5f);
                }
            }
            Destroy(this.gameObject);
        }
        else if(collision.CompareTag("Enemy"))
        {
            collision.GetComponentInParent<Enemy_Basic>().TakeDamage(Arrow_Damage);
            Destroy(this.gameObject);
        }
        else if(collision.CompareTag("Walls"))
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator Time_Destroy()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(this.gameObject);
    }
}
