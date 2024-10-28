using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Manager : MonoBehaviour
{
    public CinemachineVirtualCamera virtual_Camera;
    public CinemachineConfiner confiner;

    public void Update_Confiner(Collider2D new_Boundary)
    {
        if (confiner != null)
        {
            confiner.m_BoundingShape2D = new_Boundary;
            confiner.InvalidatePathCache();
        }
    }
}
