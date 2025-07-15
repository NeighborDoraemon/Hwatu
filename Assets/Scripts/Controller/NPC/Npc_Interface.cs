using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface Npc_Interface
{
    public void Npc_Interaction_Start();
    public void Event_Start();
    public void Npc_Interaction_End();
    public void Event_Move(InputAction.CallbackContext ctx);
    public void Event_Move_Direction(Vector2 dir);
    public void Event_Attack(InputAction.CallbackContext ctx);
}
