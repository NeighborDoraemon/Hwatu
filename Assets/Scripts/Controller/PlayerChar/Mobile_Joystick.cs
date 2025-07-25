using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mobile_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Handle Object")]
    [SerializeField] private RectTransform handle;

    [Header("Maximum Range")]
    [SerializeField] private float handle_Range = 100.0f;

    [Header("Player Controller")]
    [SerializeField] private PlayerCharacter_Controller player;

    private Vector2 input_Direction;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        Vector2 clamped = Vector2.ClampMagnitude(localPoint, handle_Range);
        input_Direction = clamped / handle_Range;

        handle.anchoredPosition = clamped;

        player.On_Move_Input(new Vector2(input_Direction.x, input_Direction.y));

        if (input_Direction.y < -0.5f)
            player.On_Down_Button_Down();
        else
            player.On_Down_Button_Up();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input_Direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        player.On_Move_Stop();
    }
}
