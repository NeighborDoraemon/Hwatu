using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy_Parent : MonoBehaviour
{
    [Header("Parent Value")]
    [SerializeField] protected GameObject Target_Player;
    [SerializeField] protected BoolReference BR_Facing_Left;
    [SerializeField] protected BoolReference BR_Stunned;

    protected void TurnAround()
    {
        Quaternion quater = this.gameObject.transform.rotation;

        if (this.gameObject.transform.position.x <= Target_Player.transform.position.x && BR_Facing_Left.Value) // 좌측 보는중 & 플레이어가 우측
        {
            BR_Facing_Left.Value = false;
            quater.y = 180.0f;

            this.gameObject.transform.rotation = quater;
        }
        else if (this.gameObject.transform.position.x > Target_Player.transform.position.x && !BR_Facing_Left.Value)
        {
            BR_Facing_Left.Value = true;
            //Obj_Enemy.gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            quater.y = 0.0f;

            this.gameObject.transform.rotation = quater;
        }
    }

    protected IEnumerator Stun_Durable(float Duration)
    {
        yield return new WaitForSeconds(Duration);
        BR_Stunned.Value = false;
    }

    public void Take_Stun(float duration)
    {
        BR_Stunned.Value = true;

        StartCoroutine(Stun_Durable(duration));
    }
}
