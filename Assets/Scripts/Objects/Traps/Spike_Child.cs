using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Child : MonoBehaviour
{
    private Trap_Box trapBox;

    private void Awake()
    {
        trapBox = GetComponentInParent<Trap_Box>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void is_Once()
    {
        trapBox.Once_End();
    }

    public void Call_Damage()
    {
        trapBox.On_Damage();
    }
}