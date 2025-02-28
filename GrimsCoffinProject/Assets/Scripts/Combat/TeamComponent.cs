using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TeamIndex
{
    None = -1,
    Neutral = 0,
    Player,
    Enemy,
    Count,
    BreakWall,
    Rope
}

public class TeamComponent : MonoBehaviour
{
    [SerializeField] private TeamIndex _teamIndex = TeamIndex.None;

    public TeamIndex teamIndex
    {
        set
        {
            if (_teamIndex == value)
            {
                return;
            }

            _teamIndex = value;
        }

        get { return _teamIndex; }
    }
}
