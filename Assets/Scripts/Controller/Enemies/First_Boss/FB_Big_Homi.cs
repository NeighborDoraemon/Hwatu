using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_Big_Homi : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject peasent;

    [Header("Values")]
    [SerializeField] private int i_Damage;
    [SerializeField] private bool is_Reverse;

    private void Start()
    {
        peasent = FindObjectOfType<FB_Peasent>().gameObject;
    }

    private void FixedUpdate()
    {
        if (is_Reverse) // Big_Homi_Reverse
        {
            this.gameObject.GetComponent<Rigidbody2D>().AddTorque(-0.02f, ForceMode2D.Impulse);
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().AddTorque(0.02f, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Big Homi");
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Damage);
        }

        if(collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("OneWayPlatform"))
        {
            peasent.gameObject.GetComponent<FB_Peasent>().is_Homi_Crashed = true;
            Destroy(gameObject);
        }
    }
}
