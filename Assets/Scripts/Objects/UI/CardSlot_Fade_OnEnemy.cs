using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot_Fade_OnEnemy : MonoBehaviour
{
    [Header("투명도 조절할 CanvasGroup")]
    public CanvasGroup cardSlots_Group;

    [Header("페이드 값")]
    [Range(0.0f, 1.1f)] public float visible_Alpha = 1.0f;
    [Range(0.0f, 1.1f)] public float faded_Alpha = 0.4f;

    private void Update()
    {
        bool isOverlapping = false;
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Vector2 screen_Pos = RectTransformUtility.WorldToScreenPoint(Camera.main, enemy.transform.position);
            if (RectTransformUtility.RectangleContainsScreenPoint(
                cardSlots_Group.GetComponent<RectTransform>(),
                screen_Pos,
                null))
            {
                isOverlapping = true;
                break;
            }
        }

        cardSlots_Group.alpha = isOverlapping ? faded_Alpha : visible_Alpha;
    }
}
