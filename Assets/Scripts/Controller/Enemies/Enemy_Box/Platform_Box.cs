using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Box : MonoBehaviour
{
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Chasing;

    private int Platform_Index;
    private bool initialized = false;

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
            // 벽에 닿았으면 즉시 방향 전환
            BR_At_End.Value = true;
            return;
        }

        if (collision.CompareTag("Platform") || collision.CompareTag("OneWayPlatform"))
        {
            Platform_Index++;

            if (!initialized)
            {
                initialized = true; // 첫 발판 접촉으로 초기화
                return;
            }

            // 발판에 정상적으로 접촉 중이면 끝이 아님
            BR_At_End.Value = false;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform") || collision.CompareTag("OneWayPlatform"))
        {
            Platform_Index--;

            if (initialized && Platform_Index <= 0)
            {
                // 더 이상 발판에 닿아있지 않음
                BR_At_End.Value = true;
                BR_Chasing.Value = false;
            }
        }
    }
}
