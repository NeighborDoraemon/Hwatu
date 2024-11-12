using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_Mini_Homi : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private int i_Damage;
    [SerializeField] private float f_Back_Power;
    [SerializeField] private float f_Throw_Distance;

    [Header("Objects")]
    [SerializeField] private GameObject peasent;

    [Header("Others")]
    [SerializeField] private Rigidbody2D homi_rigid;


    private Vector3 Start_Position;
    private bool is_Going_Left = false;

    // Start is called before the first frame update
    void Start()
    {
        peasent = FindObjectOfType<FB_Peasent>().gameObject;
        Start_Position = this.gameObject.transform.position;

        if (Start_Position.x < peasent.transform.position.x)
        {
            is_Going_Left = true;
            homi_rigid.AddForce(new Vector2(-30.0f * f_Back_Power, 0.0f));
        }
        else
        {
            is_Going_Left = false;
            homi_rigid.AddForce(new Vector2(30.0f * f_Back_Power, 0.0f));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(is_Going_Left) // Going Left
        {
            //if (homi_rigid.velocity.x <= 0.1f)
            //{
            //    homi_rigid.velocity = new Vector2(4.0f, 0.0f);
            //}
            //else
            //{
                homi_rigid.AddForce(new Vector2(f_Back_Power, 0.0f));
            //}
        }
        else
        {
            //if (homi_rigid.velocity.x <= 0.1f)
            //{
            //    homi_rigid.velocity = new Vector2(-4.0f, 0.0f);
            //}
            //else
            //{
                homi_rigid.AddForce(new Vector2(-f_Back_Power, 0.0f));
            //}
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Damage);
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<FB_Peasent>().is_Homi_Back = true;
            Destroy(gameObject);
        }
    }
}
