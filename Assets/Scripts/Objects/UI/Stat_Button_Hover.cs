using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Stat_Button_Hover : MonoBehaviour, IPointerEnterHandler
{
    [Header("Stat Npc Controller")]
    public Stat_Npc_Controller stat_Con;
    [Header("Button Index")]
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        stat_Con.On_Stat_Hover(index);
    }
}
