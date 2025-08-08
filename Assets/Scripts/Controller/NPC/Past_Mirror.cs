using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Past_Mirror : MonoBehaviour, Npc_Interface
{
    [SerializeField] private Map_Manager map_Manager;

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
    }
}
