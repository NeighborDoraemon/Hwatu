using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Clone : MonoBehaviour
{
    public PlayerCharacter_Controller player;
    public Weapon_Collision_Handler weapon_Handler;
    public SpriteRenderer effect_Render;
    public Animator effect_Animator;
    public Weapon_Data cur_Weapon_Data;

    public float follow_Speed = 10.0f;
    public float attack_Delay = 0.2f;
    public bool is_Facing_Right { get { return player != null ? player.is_Facing_Right : true; } } 

    private Animator clone_Animator;
    private GameObject cur_Weapon;
    private Transform weapon_Holder;

    public void Initialize(PlayerCharacter_Controller player)
    {
        this.player = player;

        clone_Animator = GetComponent<Animator>();

        if (player != null && player.GetComponent<Animator>() != null)
        {
            clone_Animator.runtimeAnimatorController = player.GetComponent<Animator>().runtimeAnimatorController;
        }

        weapon_Holder = new GameObject("Clone_Weapon_Holder").transform;
        weapon_Holder.SetParent(transform);
        weapon_Holder.localPosition = Vector3.zero;
        weapon_Holder.localScale = Vector3.one;

        GameObject effectRenderObj = new GameObject("Clone_Effect_Render");
        effectRenderObj.transform.SetParent(transform);
        effectRenderObj.transform.localPosition = Vector3.zero;
        effectRenderObj.transform.localScale = Vector3.one;

        effect_Render = effectRenderObj.AddComponent<SpriteRenderer>();
        effect_Animator = effectRenderObj.AddComponent<Animator>();

        Copy_Player_Weapon();
    }

    private void Update()
    {
        if (player == null) return;

        Sync_Animation();
    }

    private void LateUpdate()
    {
        Follow_Player();
        Update_WeaponHolder_Position();
    }

    void Update_WeaponHolder_Position()
    {
        if (player == null || player.weapon_Anchor == null || weapon_Holder == null)
        {
            return;
        }

        Vector3 new_Pos = player.weapon_Anchor.localPosition;
        Quaternion new_Rotation = player.weapon_Anchor.localRotation;
        Vector3 new_Scale = player.weapon_Anchor.localScale;

        if (player.is_Facing_Right)
        {
            weapon_Holder.localPosition = new_Pos;
            weapon_Holder.localRotation = new_Rotation;
            weapon_Holder.localScale = new_Scale;
        }
        else
        {
            weapon_Holder.localPosition = new Vector3(-new_Pos.x, new_Pos.y, new_Pos.z);
            weapon_Holder.localRotation = Quaternion.Inverse(new_Rotation);
            weapon_Holder.localScale = new Vector3(-new_Scale.x, new_Scale.y, new_Scale.z);
        }
    }

    public void Activate_WeaponCollider(float duration)
    {
        if (weapon_Handler != null)
        {
            weapon_Handler.Enable_Collider(duration);
        }
    }

    public void Show_Effect(string motion_Name_And_Frame)
    {
        string[] parts = motion_Name_And_Frame.Split(',');
        if (parts.Length < 2) return;

        string motion_Name = parts[0];
        int frame_Num;
        if (!int.TryParse(parts[1], out frame_Num))
            return;

        if (cur_Weapon_Data == null || cur_Weapon_Data.effect_Data == null)
            return;

        var effect_Info = cur_Weapon_Data.effect_Data.Get_Effect_Info(motion_Name);
        if (effect_Info != null)
        {
            var frame_Effect_Info = effect_Info.frame_Effects.Find(fe => fe.frame_Number == frame_Num);
            if (frame_Effect_Info != null)
            {
                Vector3 effect_Pos = frame_Effect_Info.position_Offset;
                effect_Pos.x = is_Facing_Right ? Mathf.Abs(effect_Pos.x) : -Mathf.Abs(effect_Pos.x);

                if (effect_Render != null)
                {
                    effect_Render.transform.localPosition = effect_Pos;
                    effect_Render.transform.localScale = new Vector3(is_Facing_Right ? 1 : -1, 1, 1);
                }

                if (frame_Effect_Info.effect_Animator != null)
                {
                    if (effect_Animator != null)
                    {
                        effect_Animator.runtimeAnimatorController = frame_Effect_Info.effect_Animator;
                        effect_Animator.Play("Effect_Start");
                        StartCoroutine(Reset_Effect_After_Animation(frame_Effect_Info.duration));
                    }
                }
                else if (frame_Effect_Info.effect_Sprites != null)
                {
                    if (effect_Render != null)
                    {
                        effect_Render.sprite = frame_Effect_Info.effect_Sprites;
                        effect_Render.enabled = true;
                        Invoke("HideEffect", frame_Effect_Info.duration);
                    }
                }
            }
        }
    }

    private IEnumerator Reset_Effect_After_Animation(float duration)
    {
        yield return new WaitForSeconds(duration);
        effect_Animator.runtimeAnimatorController = null;
        HideEffect();
    }

    public void HideEffect()
    {
        effect_Render.enabled = false;
        effect_Render.sprite = null;
    }

    private void Follow_Player()
    {
        Vector3 target_Pos = player.transform.position + new Vector3(-0.5f * (player.is_Facing_Right ? 1 : -1), 0, 0);
        transform.position = Vector3.Lerp(transform.position, target_Pos, follow_Speed * Time.deltaTime);

        transform.localScale = new Vector3(player.is_Facing_Right ? 2 : -2, 2, 1);
    }

    private void Sync_Animation()
    {
        if (player == null || clone_Animator == null) return;

        Animator player_Animator = player.GetComponent<Animator>();
        foreach (AnimatorControllerParameter param in player_Animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                clone_Animator.SetBool(param.name, player_Animator.GetBool(param.name));
            }
            else if (param.type == AnimatorControllerParameterType.Float)
            {
                clone_Animator.SetFloat(param.name, player_Animator.GetFloat(param.name));
            }
            else if (param.type == AnimatorControllerParameterType.Int)
            {
                clone_Animator.SetInteger(param.name, player_Animator.GetInteger(param.name));
            }
            else if (param.type == AnimatorControllerParameterType.Trigger)
            {
                if (player_Animator.GetBool(param.name))
                {
                    clone_Animator.SetTrigger(param.name);
                }
            }
        }
    }

    public void Copy_Player_Weapon()
    {
        if (player == null || player.cur_Weapon_Data == null)
        {
            Debug.LogWarning("플레이어나 무기 데이터가 존재하지 않음");
            return;
        }
        
        cur_Weapon_Data = player.cur_Weapon_Data;

        if (cur_Weapon != null)
        {
            Destroy(cur_Weapon);
        }

        foreach (Transform child in weapon_Holder)
        {
            Destroy(child.gameObject);
        }

        if (player != null && player.GetComponent<Animator>() != null)
        {
            clone_Animator.runtimeAnimatorController = player.GetComponent<Animator>().runtimeAnimatorController;
        }

        if (player.cur_Weapon_Data.weapon_Prefab != null)
        {
            cur_Weapon = Instantiate(player.cur_Weapon_Data.weapon_Prefab, weapon_Holder);
            cur_Weapon.transform.localPosition = Vector3.zero;
            cur_Weapon.transform.localRotation = Quaternion.identity;

            weapon_Handler = cur_Weapon.GetComponent<Weapon_Collision_Handler>();

            Animator weapon_Animator = cur_Weapon.GetComponent<Animator>();
            if (weapon_Animator != null && player.cur_Weapon_Data.weapon_overrideController != null)
            {
                weapon_Animator.runtimeAnimatorController = player.cur_Weapon_Data.overrideController;
            }
        }
    }
}
