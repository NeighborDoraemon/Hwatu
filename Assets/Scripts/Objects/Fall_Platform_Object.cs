using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall_Platform_Object : MonoBehaviour
{
    [SerializeField] private Vector3 v_Return_Position;

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
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = v_Return_Position;
        }
    }
}
