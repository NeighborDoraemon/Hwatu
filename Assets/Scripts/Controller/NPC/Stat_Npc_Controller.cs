using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat_Npc_Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter_Controller player_Con;
    [SerializeField] private Canvas Main_Can;

    [Header("Stat UI")]
    [SerializeField] private Canvas Stat_Manage_UI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UI_Start()
    {
        Main_Can.gameObject.SetActive(false);
        Stat_Manage_UI.gameObject.SetActive(true);
    }

    public void Btn_Health_Up()
    {
        player_Con.health += 10;
        player_Con.max_Health += 10;
    }

    public void Btn_Attack_Up()
    {
        player_Con.attackDamage += 2;
    }

    public void Btn_Jump_Up()
    {
        player_Con.jumpPower += 0.2f;
    }

    public void Btn_Exit()
    {
        Stat_Manage_UI.gameObject.SetActive(false);
        Main_Can.gameObject.SetActive(true);
    }
}
