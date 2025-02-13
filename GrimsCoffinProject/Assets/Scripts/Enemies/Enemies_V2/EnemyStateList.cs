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
}
