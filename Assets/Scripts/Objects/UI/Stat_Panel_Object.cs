using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat_Panel_Object : MonoBehaviour
{
    public enum Texts
    {
        Damage = 0,
        Skill_Damage = 1,
        Health = 2,
        Now_Health = 3,
        Speed = 4,
        Critical_Rate = 5,
        Critical_Damage = 6,
        Count = 7
    }

    [SerializeField] private PlayerCharacter_Stat_Manager p_Stat_Manager;

    [Header("Texts")]
    [SerializeField] private List<Text> Text_Lists = new List<Text>();

    // Start is called before the first frame update
    void Start()
    {
        Set_Stat_Panel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Set_Stat_Panel()
    {
        Text_Lists[0].text = (p_Stat_Manager.cur_Weapon_Data.attack_Damage + p_Stat_Manager.attackDamage).ToString();
        Text_Lists[1].text = (p_Stat_Manager.cur_Weapon_Data.skill_Damage + p_Stat_Manager.skill_Damage).ToString();
        Text_Lists[2].text = p_Stat_Manager.max_Health.ToString();
        Text_Lists[3].text = p_Stat_Manager.health.ToString();
        Text_Lists[4].text = (p_Stat_Manager.movementSpeed * p_Stat_Manager.movementSpeed_Mul).ToString();
        float crit_Percent = p_Stat_Manager.crit_Rate * 100.0f;
        Text_Lists[5].text = crit_Percent.ToString("F0") + " %";
        Text_Lists[6].text = "x" + p_Stat_Manager.crit_Dmg.ToString();
    }
}
