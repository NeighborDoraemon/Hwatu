using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Box : MonoBehaviour
{
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Chasing;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Walls"))
        {
            BR_At_End.Value = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "OneWayPlatform")
        {
            BR_At_End.Value = true;
            BR_Chasing.Value = false;
        }
    }
}
