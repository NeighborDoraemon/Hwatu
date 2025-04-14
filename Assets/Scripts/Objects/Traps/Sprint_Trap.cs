using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint_Trap : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private float Power;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal;
            Debug.Log("Normal: " + normal);

            if (normal.y < -0.5f)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().velocity += new Vector2(0.0f, Power);
            }
        }
    }
}
