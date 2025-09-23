using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Porjectile : MonoBehaviour
{
    [SerializeField] public int i_Projectile_Damage;
    [SerializeField] private bool is_Bolas = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Time_Destroy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Projectile_Damage);

            if(is_Bolas) // bind
            {
                collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Bind(2.0f);
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Platform") /*|| collision.gameObject.CompareTag("OneWayPlatform")*/ || collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Time_Destroy()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(this.gameObject);
    }
}
