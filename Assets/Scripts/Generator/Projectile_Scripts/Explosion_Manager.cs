using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_Manager : MonoBehaviour
{
    public float destroy_Time;

    private void Awake()
    {
        Destroy(this.gameObject, destroy_Time);
    }
}
