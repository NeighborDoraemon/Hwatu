using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Energy_Ball : MonoBehaviour
{
    [SerializeField] private GameObject Target_Player;
    [SerializeField]private float f_speed = 2.0f;
    [SerializeField] private float Turn_Speed = 2.0f;

    private Vector3 direction;

    public void Get_Data(GameObject player, float Speed)
    {
        Target_Player = player;
        f_speed = Speed;
        direction = transform.forward;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Target_Player != null)
        {
            // 목표 방향 계산
            Vector3 targetDir = (Target_Player.transform.position - transform.position).normalized;
            targetDir.y = targetDir.y -  0.25f; // Y축 보정 (플레이어의 위치에 따라 조정 필요)

            // 보간으로 방향 전환 (약하게)
            direction = Vector3.Lerp(direction, targetDir, Turn_Speed * Time.deltaTime);
        }

        // 이동
        transform.position += direction * f_speed * Time.deltaTime;
    }
}
