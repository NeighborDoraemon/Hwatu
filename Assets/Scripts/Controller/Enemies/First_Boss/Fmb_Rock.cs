using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fmb_Rock : MonoBehaviour
{
    [SerializeField] public int i_Projectile_Damage;
    [HideInInspector]public bool is_Big_Rock = true;

    [SerializeField] private int i_Rock_Count = 2;
    [SerializeField] private float f_small_Rock_Power = 5.0f;
    [SerializeField] private float f_small_Rock_Angle = 30.0f;
    [SerializeField] private Vector3 f_small_Rock_Scale = new Vector3(0.5f, 0.5f, 1.0f);

    [SerializeField] private GameObject Rock_Prefab;

    private bool is_Collider_Enabled = false;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Time_Destroy());
        StartCoroutine(Small_Rock_Enable());
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

            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("Walls"))
        {
            if (is_Big_Rock)
            {

                GameObject leftRock = Instantiate(Rock_Prefab, transform.position, Quaternion.identity);

                leftRock.GetComponent<Fmb_Rock>().is_Big_Rock = false;
                leftRock.transform.localScale = f_small_Rock_Scale;
                // 왼쪽 방향으로 힘을 추가하여 포물선 궤적 생성
                Vector2 leftDirection = Quaternion.Euler(0, 0, 0 + f_small_Rock_Angle) * Vector2.up;
                leftRock.GetComponent<Rigidbody2D>().AddForce(leftDirection * f_small_Rock_Power, ForceMode2D.Impulse);

                // 오른쪽으로 발사될 작은 바위 생성
                GameObject rightRock = Instantiate(Rock_Prefab, transform.position, Quaternion.identity);

                rightRock.GetComponent<Fmb_Rock>().is_Big_Rock = false;
                rightRock.transform.localScale = f_small_Rock_Scale;
                // 오른쪽 방향으로 힘을 추가하여 포물선 궤적 생성
                Vector2 rightDirection = Quaternion.Euler(0, 0, 0 - f_small_Rock_Angle) * Vector2.up;
                rightRock.GetComponent<Rigidbody2D>().AddForce(rightDirection * f_small_Rock_Power, ForceMode2D.Impulse);


                Destroy(gameObject);
            }
            else
            {
                // 작은 바위는 충돌유예
                if (is_Collider_Enabled)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private IEnumerator Time_Destroy()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(this.gameObject);
    }

    private IEnumerator Small_Rock_Enable()
    {
        yield return new WaitForSeconds(0.5f);
        is_Collider_Enabled = true;
    }
}
