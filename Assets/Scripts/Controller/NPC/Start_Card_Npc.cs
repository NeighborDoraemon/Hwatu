using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Start_Card_Npc : MonoBehaviour, Npc_Interface
{
    [SerializeField] private Transform[] Card_Spawn_Points;
    [SerializeField] private Animator npc_Animator;
    private int Spawn_Count = 2;

    private PlayerCharacter_Controller player;

    public bool give_Card = false;

    [SerializeField] private float random_Motion_Interval_Min = 3.0f;
    [SerializeField] private float random_Motion_Interval_Max = 7.0f;

    [Header("Dialogue Index")]
    [SerializeField] private int Interaction_start;
    [SerializeField] private int After_Event;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
    }

    private void Start()
    {
        StartCoroutine(Random_Motion_Routine());
    }

    public void Request_Spawn_Cards()
    {
        if (Object_Manager.instance == null)
        {
            Debug.LogError("Card Box에서 Card_Spawner 인스턴스 실종");
            return;
        }

        int spawnCount = Mathf.Min(Spawn_Count, Card_Spawn_Points.Length);

        if (!give_Card)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 spawnPos = Card_Spawn_Points[i].position;
                GameObject spawned_Card = Object_Manager.instance.Spawn_Cards(spawnPos);

                if (spawned_Card != null)
                {
                    player.AddCard(spawned_Card);
                }
            }
        }

        give_Card = true;
    }


    private IEnumerator Random_Motion_Routine()
    {
        while (true)
        {
            float wait_Time = UnityEngine.Random.Range(random_Motion_Interval_Min, random_Motion_Interval_Max);
            yield return new WaitForSeconds(wait_Time);

            int random_Motion = UnityEngine.Random.Range(0, 2);

            if (random_Motion == 0)
            {
                npc_Animator.SetTrigger("Wind_Trigger");
            }
            else
            {
                npc_Animator.SetTrigger("Pose_Trigger");
            }
        }
    }

    public void Npc_Interaction_Start()
    {
        if (player != null && !give_Card)
        {
            player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
            Dialogue_Manager.instance.Start_Dialogue(Interaction_start);
        }
        else if (player != null && give_Card)
        {
            player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
            Dialogue_Manager.instance.Start_Dialogue(After_Event);
        }
    }
    public void Event_Start()   //Not used
    {
    }
    public void Npc_Interaction_End()
    {
        if (player != null)
        {
            player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
            player.Event_State_Change(PlayerCharacter_Controller.Event_State.None);

            if (!give_Card)
            {
                Request_Spawn_Cards();
            }
        }
    }

    public void Event_Move(InputAction.CallbackContext ctx) //Not used
    {
    }
    public void Event_Attack(InputAction.CallbackContext ctx)   //Not used
    {
    }
}
