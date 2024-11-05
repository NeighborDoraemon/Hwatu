using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [Header("Delay")]
    [SerializeField] private float f_Delay = 3.0f;


    [SerializeField] private GameObject Obj_HealBox;

    private float f_Attack_Time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        f_Attack_Time += Time.deltaTime;

        Call_Heal();
    }


    private void Call_Heal()
    {
        if(f_Attack_Time >= f_Delay)
        {
            Debug.Log("Heal Called");
            Obj_HealBox.GetComponent<Heal_Box>().Heal();
            f_Attack_Time = 0.0f;
        }
    }
}
