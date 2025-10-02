using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Noose_Trap : MonoBehaviour
{
    [SerializeField] private GameObject bind_spot;
    [SerializeField] private float bind_time;

    [SerializeField] private GameObject Keyboard_Object;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(collision.GetComponent<PlayerCharacter_Controller>().is_Knock_Back)
            {
                return;
            }
            collision.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
            collision.gameObject.transform.position = bind_spot.transform.position;
            collision.GetComponent<PlayerCharacter_Controller>().Player_Trap_Stun(false, () =>
            {
                StartCoroutine(Reset_Values(collision.gameObject));
            });
            
            Keyboard_Object.SetActive(true);

            //StartCoroutine(Reset_Values(collision.gameObject));
        }
    }

    IEnumerator Reset_Values(GameObject other)
    {
        yield return new WaitForSeconds(bind_time);

        other.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        Destroy(this.gameObject);
    }
}
