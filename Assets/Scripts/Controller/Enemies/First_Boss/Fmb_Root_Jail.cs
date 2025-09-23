using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fmb_Root_Jail : MonoBehaviour
{
    [SerializeField] private GameObject Jail_Empty;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Rise up
        if (Jail_Empty != null && Jail_Empty.transform.position.y < this.transform.position.y)
        {
            Jail_Empty.transform.Translate(new Vector3(0.0f, 2.0f * Time.deltaTime, 0.0f));
        }
        else
        {
            Jail_Empty.transform.position = this.transform.position;
        }
    }
}
