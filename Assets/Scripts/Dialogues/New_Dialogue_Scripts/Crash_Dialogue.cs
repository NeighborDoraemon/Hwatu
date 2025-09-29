using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Crash_Dialogue : MonoBehaviour, Npc_Interface
{
    [SerializeField] private int dialogue_Index;
    [SerializeField] private int Print_time = 1;
    private PlayerCharacter_Controller p_Controller;

    public void Event_Attack(InputAction.CallbackContext ctx){}
    public void Event_Move(InputAction.CallbackContext ctx){}
    public void Event_Move_Direction(Vector2 dir){}

    public void Event_Start()
    {
    }

    public void Npc_Interaction_End()
    {
        if(p_Controller == null) { return; }
        p_Controller.State_Change(PlayerCharacter_Controller.Player_State.Normal);
    }

    public void Npc_Interaction_Start()
    {
        if(p_Controller == null) { return; }
        p_Controller.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        // Set player stop move
        p_Controller.Player_Vector_Stop();
        Dialogue_Manager.instance.Start_Dialogue(dialogue_Index);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Print_time--;
            if (Print_time < 0) { return; }
            p_Controller = collision.GetComponent<PlayerCharacter_Controller>();
            Dialogue_Manager.instance.Get_Npc_Data(this.gameObject);
            Npc_Interaction_Start();
        }
    }
}
