using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Porjectile : MonoBehaviour
{
    [SerializeField] private int i_Projectile_Damage;

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
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Projectile_Damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("OneWayPlatform") || collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
