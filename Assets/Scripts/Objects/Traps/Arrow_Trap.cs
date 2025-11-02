using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Trap : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private float arrow_speed;
    [SerializeField] private int arrow_damage;

    [SerializeField] private List<Arrow_Trap_List> arrow_list = new List<Arrow_Trap_List>();

    [SerializeField] private Animator trap_Animator;

    private bool is_Once = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (!is_Once)
            {
                StartCoroutine(Spawn_Arrow());
                if (trap_Animator != null)
                {
                    trap_Animator.SetTrigger("On_Trigger");
                }
            }
        }
    }

    IEnumerator Spawn_Arrow()
    {
        yield return new WaitForSeconds(1.0f);
        is_Once = true;

        if (arrow != null)
        {
            for (int i = 0; i < arrow_list.Count; i++)
            {
                GameObject arrow_prefab = Instantiate(arrow, arrow_list[i].Trap.transform.position, Quaternion.identity);
                arrow_prefab.GetComponent<Arrow_Trap_Projectile>().Arrow_Damage = arrow_damage;

                if (arrow_list[i].is_Left)
                {
                    arrow_prefab.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f * arrow_speed, 0.0f);
                }
                else
                {
                    arrow_prefab.GetComponent<Rigidbody2D>().velocity = new Vector2(1.0f * arrow_speed, 0.0f);
                    arrow_prefab.GetComponent<SpriteRenderer>().flipX = true;
                }
            }
        }

        yield return new WaitForSeconds(2.0f);
        is_Once = false;
    }
}

[System.Serializable]
public class Arrow_Trap_List
{
    public GameObject Trap;
    public bool is_Left;
}
