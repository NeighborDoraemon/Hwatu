using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Market_Npc : MonoBehaviour, Npc_Interface
{
    [Header("Dialogue Index")]
    [SerializeField] private int Interaction_First;
    [SerializeField] private int Interaction_After;

    private bool is_First_Interaction = true;
    [Header("Others")]
    [SerializeField] private PlayerCharacter_Controller player;

    public void Event_Attack(InputAction.CallbackContext ctx){}
    public void Event_Move(InputAction.CallbackContext ctx){}
    public void Event_Move_Direction(Vector2 dir){}

    public void Event_Start()
    {
    }

    public void Npc_Interaction_End()
    {
        if (player != null)
        {
            player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
        }
    }

    public void Npc_Interaction_Start()
    {
        player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        player.Player_Vector_Stop();

        if (is_First_Interaction)
        {
            Dialogue_Manager.instance.Start_Dialogue(Interaction_First);
            is_First_Interaction = false;
        }
        else
        {
            Dialogue_Manager.instance.Start_Dialogue(Interaction_After);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
