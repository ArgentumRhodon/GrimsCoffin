using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateList : MonoBehaviour
{
    //Variables track what state the enemy currently is in
    public bool IsFacingRight { get; set; }
    public bool IsJumping { get; set; }
    public bool IsDashing { get; set; }
    public bool IsSeeking { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsLooking { get; set; }
    public bool IsIdle { get; set; }
    public bool IsSleeping { get; set; }
    public bool IsHitDown { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsStaggered { get; set; }
    public bool IsDamaged { get; set; }
    public bool IsDead { get; set; }

}
