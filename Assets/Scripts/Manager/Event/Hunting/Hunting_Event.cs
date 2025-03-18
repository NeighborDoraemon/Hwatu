using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunting_Event : MonoBehaviour
{
    [SerializeField] private List<GameObject> Spawn_Points = new List<GameObject>();
    [SerializeField] private List<GameObject> Bird_Prefabs = new List<GameObject>();

    private PlayerCharacter_Controller p_controller = null;

    [HideInInspector] public int Prf_Count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Spawn_Birds()
    {
        Prf_Count = 0;
        for(int i = 0; i < Spawn_Points.Count; i++)
        {
            if(i == 6 || i == 9)
            {
                Prf_Count++;
            }

            GameObject bird_pref = Instantiate(Bird_Prefabs[Prf_Count], Spawn_Points[i].transform.position, Quaternion.identity);

            bird_pref.GetComponent<Hunting_Bird>().Set_Manager(this.gameObject);
            bird_pref.GetComponent<Hunting_Bird>().Set_Position(Spawn_Points[i].transform.position);
        }        
    }

    public void Hunting_Dialogue_Start()
    {
        if(p_controller != null)
        {
            p_controller.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            p_controller = collision.gameObject.GetComponent<PlayerCharacter_Controller>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            p_controller = null;
        }
    }
}
