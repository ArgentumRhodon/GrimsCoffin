using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    //Variables control the actions the player can perform at any time.
    public bool IsFacingRight { get; set; }
    public bool IsJumping { get; set; }
    public bool IsWallJumping { get; set; }
    public bool IsDashing { get; set; }
    public bool IsSliding { get; set; }


}