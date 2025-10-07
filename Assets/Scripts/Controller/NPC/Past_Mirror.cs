using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Past_Mirror : MonoBehaviour, Npc_Interface
{
    [SerializeField] private Map_Manager map_Manager;
    [SerializeField] private List<GameObject> Cr_Dials = new List<GameObject>();
    [SerializeField] private bool is_from_Start = false;
    [SerializeField] private GameObject tutorial_box;
    [SerializeField] private Vector3 Box_Spawn_position;
    [SerializeField] private GameObject Already_Box;

    public void Event_Attack(InputAction.CallbackContext ctx)
    {
    }

    public void Event_Move(InputAction.CallbackContext ctx)
    {
    }

    public void Event_Move_Direction(Vector2 dir)
    {
    }

    public void Event_Start()
    {
    }

    public void Npc_Interaction_End()
    {
    }

    public void Npc_Interaction_Start()
    {
        Debug.Log("Interaction");
        map_Manager.Move_Tutorial();

        foreach (var dial in Cr_Dials)
        {
            if (dial != null)
            {
                dial.GetComponent<Crash_Dialogue>().Reset_value();
            }
        }

        if (is_from_Start)
        {
            if (tutorial_box != null && Already_Box == null)
            {
                Instantiate(tutorial_box, Box_Spawn_position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Tutorial Box is null");
            }
        }
    }
}
