using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack_Strategy
{
    void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data);
    void Shoot(PlayerCharacter_Controller player, Transform fire_Point);
    void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data);
}
