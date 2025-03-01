using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangtae_AutoDestroy : MonoBehaviour
{
    void Start()
    {
        Animator animtor = GetComponent<Animator>();
        if (animtor != null && animtor.runtimeAnimatorController != null)
        {
            float anim_Length = animtor.runtimeAnimatorController.animationClips[0].length;
            Destroy(gameObject, anim_Length);
        }
        else
        {
            Destroy(gameObject, 0.8f);
        }
    }
}
