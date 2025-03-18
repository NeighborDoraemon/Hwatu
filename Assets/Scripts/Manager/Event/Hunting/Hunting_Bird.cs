using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunting_Bird : MonoBehaviour
{
    private GameObject Event_Manager;

    private float Spawned_Position;
    private float Max_Position;
    private float Min_Position;


    [SerializeField] private float Move_Speed;
    [SerializeField] private float Move_Area;

    private Color gizmo_color = Color.red;

    private Rigidbody2D bird_rigid;
    [SerializeField] private bool is_Facing_Left; //시작 방향

    public void Set_Manager(GameObject Object)
    {
        Event_Manager = Object;
    }

    public void Set_Position(Vector3 vector)
    {
        Spawned_Position = vector.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        bird_rigid = this.gameObject.GetComponent<Rigidbody2D>();
        //Bird_Move();

        Set_Position(this.gameObject.transform.position);
        Set_Area();
    }

    // Update is called once per frame
    void Update()
    {
        //if(this.transform.position.x >= Max_Position || this.transform.position.x <= Min_Position)
        //{
        //    is_Facing_Left = !is_Facing_Left;
        //}
        //Bird_Move();
    }

    private void Bird_Move()
    {
        bird_rigid.velocity = is_Facing_Left ? new Vector2(-1.0f * Move_Speed, 0.0f) : new Vector2(1.0f * Move_Speed, 0.0f);
    }

    private void Set_Area()
    {
        Max_Position = Spawned_Position + Move_Area;
        Debug.Log(Max_Position);
        Min_Position = Spawned_Position - Move_Area;
        Debug.Log(Min_Position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(true)
        //{

        //}

        //Event_Manager.GetComponent<Hunting_Event>().Prf_Count--;
        //Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmo_color;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(Move_Area, 1.0f));
    }
}
