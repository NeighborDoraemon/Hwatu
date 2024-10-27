using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siege_Chase_Box : MonoBehaviour
{
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private BoolReference BR_Look_Once;
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

            if(!BR_Look_Once.Value)
            {
                BR_Look_Once.Value = true;
            }
            //Debug.Log("플레이어 감지");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BR_Chasing.Value = false;
            Debug.Log("플레이어 놓침");
        }
    }
}
