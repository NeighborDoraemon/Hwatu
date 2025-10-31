using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunting_Projectile : MonoBehaviour
{
    private Vector3 Direction;
    [SerializeField] private float speed;
    [SerializeField] private float destroy_time;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(projectile_Coroutine());
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Event"))
        {
            Destroy(this.gameObject);
        }
    }

    public void Start_Rotate(float rotation)
    {
        this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotation + 90);
        //Direction = transform.up;
    }

    private IEnumerator projectile_Coroutine()
    {
        yield return new WaitForSeconds(destroy_time);
        Destroy(this.gameObject);
    }
}
