using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_Sniping : MonoBehaviour
{
    [Header("Values")]
    //[SerializeField] private int i_Damage;
    [SerializeField] private float f_Aim_Duration;
    [SerializeField] private float f_Follow_Speed;
    [SerializeField] private float f_ShootSpeed;

    [Header("Objects")]
    [SerializeField] private GameObject Obj_Player;
    [SerializeField] private CapsuleCollider2D player_Capsule;
    [SerializeField] private LineRenderer Line_Render;
    [SerializeField] private GameObject pfrb_Bullet;


    private float timer;
    private Vector3 target_Position;

    private bool is_Attacking_Now = false;
    private bool is_Once = false;

    private bool is_Shot_Once = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(is_Attacking_Now)
        {
            Attack_Sniping();
        }
    }

    public void Call_Attack()
    {
        is_Attacking_Now = true;
    }

    public void Stop_Attack()
    {
        Line_Render.enabled = false;
        is_Attacking_Now = false;
    }

    private void Attack_Sniping()
    {
        if(!is_Once)
        {
            Line_Render.enabled = true;
            timer = f_Aim_Duration;
            target_Position = Obj_Player.transform.position;

            is_Once = true;
            is_Shot_Once = false;
        }

        if(timer > 0)
        {
            timer -= Time.deltaTime;

            Line_Render.SetPosition(0, this.transform.position);

            target_Position = Vector3.Lerp(target_Position, player_Capsule.bounds.center, f_Follow_Speed * Time.deltaTime);
            //target_Position = player_Capsule.bounds.center;
            Line_Render.SetPosition(1, target_Position);
        }
        else
        {
            Line_Render.enabled = false;
            Shoot();
        }
    }

    private void Shoot()
    {
        if(!is_Shot_Once)
        {
            Vector3 direction = (target_Position - this.transform.position).normalized;

            GameObject Bullet = Instantiate(pfrb_Bullet, this.transform.position, Quaternion.identity);
            Rigidbody2D Bullet_Rigid = Bullet.GetComponent<Rigidbody2D>();

            Bullet_Rigid.velocity = direction * f_ShootSpeed;

            is_Attacking_Now = false;
            is_Once = false;

            is_Shot_Once = true;
        }
    }
}
