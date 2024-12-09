using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bomb_Explosion : MonoBehaviour
{
    private GameObject Obj_Player = null;

    [SerializeField] private int Explosion_Damage = 10;
    private int i_Position_Index = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnEnable()
    //{
    //    if (transform.parent != null)
    //    {
    //        Destroy(transform.parent.gameObject);
    //    }
    //    else
    //    {
    //        Debug.Log("Bomb Parent Missing!");
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Obj_Player = collision.gameObject;

            if(this.gameObject.transform.position.x <= Obj_Player.transform.position.x)
            {
                i_Position_Index = 1;
            }
            else
            {
                i_Position_Index = -1;
            }

            if (Obj_Player != null)
            {
                Obj_Player.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(i_Position_Index, 0.2f, 2.0f);
                Obj_Player.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(Explosion_Damage);
                Debug.Log("Player Explosion Damage");
            }
        }

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Debug.Log("Bomb Parent Missing!");
        }
    }
}
