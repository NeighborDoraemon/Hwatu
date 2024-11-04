using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile : MonoBehaviour
{
    [SerializeField] private int i_Damage;
    [SerializeField] private float f_moveSpeed;
    [SerializeField] private bool is_Left;

    private Rigidbody2D projec_Rigid;

    // Start is called before the first frame update
    void Start()
    {
        projec_Rigid = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Left)
        {
            projec_Rigid.AddForce(new Vector2(-f_moveSpeed, 0.0f));
        }
        else
        {
            projec_Rigid.AddForce(new Vector2(f_moveSpeed, 0.0f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Damage);

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform") || collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
