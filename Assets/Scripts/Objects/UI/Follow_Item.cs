using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Item : MonoBehaviour
{
    public Transform target;
    public Vector3 world_Offset;

    Canvas parent_Canvas;
    Camera cam;
    RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        parent_Canvas = GetComponentInParent<Canvas>();
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 screen_Pos = cam.WorldToScreenPoint(target.position + world_Offset);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parent_Canvas.transform as RectTransform,
            screen_Pos,
            parent_Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out Vector2 local_Pos);

        rt.localPosition = local_Pos;
    }
}
