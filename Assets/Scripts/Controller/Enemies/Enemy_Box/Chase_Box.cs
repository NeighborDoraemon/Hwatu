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
            BR_Chasing.Value = true;
            enemy_Animator.SetBool("is_Chasing", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BR_Chasing.Value = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BR_Chasing.Value = false;
            enemy_Animator.SetBool("is_Chasing", false);
        }
    }
}
