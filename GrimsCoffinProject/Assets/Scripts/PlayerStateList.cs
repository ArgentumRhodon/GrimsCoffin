using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; set; }
    public bool IsJumping { get; set; }
    public bool IsWallJumping { get; set; }
    public bool IsDashing { get; set; }
    public bool IsSliding { get; set; }


}
