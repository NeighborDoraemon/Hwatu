using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TickDamage_Projectile : MonoBehaviour
{
    [Tooltip("데미지 간의 사이시간")]
    [SerializeField] private float Tick_Time;
    [Tooltip("한번의 데미지")]
    [SerializeField] private int Damage_Per_Tick;
    [Tooltip("데미지 횟수")]
    [SerializeField] private int Tick_Count;

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
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Start_Tick_Damage(Tick_Time,Damage_Per_Tick,Tick_Count);

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("OneWayPlatform") || collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
