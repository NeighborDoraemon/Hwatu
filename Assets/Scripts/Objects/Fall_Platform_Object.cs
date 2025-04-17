using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall_Platform_Object : MonoBehaviour
{
    [SerializeField] private Vector3 v_Return_Position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(10);
            collision.gameObject.transform.position = v_Return_Position;
        }

        if(collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("적 낙사 호출시작");
            Enemy_Basic enemy = collision.GetComponent<Enemy_Basic>()
                            ?? collision.GetComponentInParent<Enemy_Basic>()
                            ?? collision.GetComponentInChildren<Enemy_Basic>();

            Debug.Log("적 낙사 호출완료");
            enemy.TakeDamage(999);
        }
    }
}
