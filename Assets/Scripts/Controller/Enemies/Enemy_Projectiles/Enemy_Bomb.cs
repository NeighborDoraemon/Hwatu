using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bomb : MonoBehaviour
{
    [SerializeField] private Rigidbody2D bomb_Rigid;
    [SerializeField] private GameObject Obj_Explosion;

    [SerializeField] private float f_Throw_Power = 2.0f;
    [SerializeField] private Vector2 v_Degree;
    private float f_Start_Y_position = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        f_Start_Y_position = this.gameObject.transform.position.y; 
        bomb_Rigid.AddForce(v_Degree.normalized * f_Throw_Power, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform") || collision.CompareTag("OneWayPlatform"))
        {
            if (this.gameObject.transform.position.y <= f_Start_Y_position)
            {
                Obj_Explosion.SetActive(true);
            }
        }
        else if(collision.CompareTag("Player"))
        {
            Obj_Explosion.SetActive(true);
        }
    }
}
