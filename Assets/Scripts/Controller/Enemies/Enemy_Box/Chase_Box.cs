using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Box : MonoBehaviour
{
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private Animator enemy_Animator;
    //private GameObject Target_Player;


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
            //Target_Player = collision.gameObject;
            //player_Object = collision.gameObject;

            BR_Chasing.Value = true;
            //Debug.Log("플레이어 감지");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BR_Chasing.Value = false;
            enemy_Animator.SetBool("is_Chasing", false);
            //Debug.Log("플레이어 놓침");
        }
    }
}
