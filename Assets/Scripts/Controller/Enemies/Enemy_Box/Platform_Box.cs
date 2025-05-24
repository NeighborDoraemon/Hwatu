using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Box : MonoBehaviour
{
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Chasing;

    private int Platform_Index;

    // Start is called before the first frame update
    void Start()
    {
        //Collider2D[] results = new Collider2D[10];
        //ContactFilter2D filter = new ContactFilter2D();
        //filter.useTriggers = true; // 트리거 검사
        //filter.SetLayerMask(LayerMask.GetMask("Platform", "OneWayPlatform"));

        //int count = GetComponent<Collider2D>().OverlapCollider(filter, results);

        //Platform_Index = count;
        //Debug.Log(count);
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
        else if(collision.CompareTag("Platform") || collision.CompareTag("OneWayPlatform"))
        {
            Platform_Index++;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "OneWayPlatform")
        {
            Platform_Index--;
            Debug.Log(Platform_Index);
            if (Platform_Index <= 0)
            {
                BR_At_End.Value = true;
                BR_Chasing.Value = false;
            }
        }
    }
}
