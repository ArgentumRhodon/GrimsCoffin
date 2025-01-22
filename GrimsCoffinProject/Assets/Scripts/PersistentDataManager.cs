using Pathfinding.Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; private set; }

    //Last Saved Location to spawn the player at when loading the game
    public Vector2 SpawnPoint { get { return new Vector2(PlayerPrefs.GetFloat("XSpawnPos", defaultXPos), PlayerPrefs.GetFloat("YSpawnPos", defaultYPos)); } }

    //Last Saved Scene the player was in when they saved the game
    public string LastSavedScene { get { return PlayerPrefs.GetString("SceneSave", defaultSceneName); } }

    //Last Saved Room the player was in when they saved the game
    public int LastSavedRoomIndex { get { return PlayerPrefs.GetInt("RoomIndex", 0); } }

    //Whether or not this is the player's first time spawning into the game
    public bool FirstSpawn { get { return PlayerPrefs.GetInt("FirstSpawn", 0) == 1; } }

    //Player Stat Values
    public float MaxHP { get { return PlayerPrefs.GetFloat("MaxHP", defaultHP); } }
    public float MaxSP { get { return PlayerPrefs.GetFloat("MaxSP", 0); } }
    public float DamageMultiplier { get { return PlayerPrefs.GetFloat("DamageMultiplier"); } }

    //Player Ability Unlocks
    public bool CanDoubleJump { get { return PlayerPrefs.GetInt("CanDoubleJump", 0) == 1; } }
    public bool CanDash { get { return PlayerPrefs.GetInt("CanDash", 0) == 1; } }
    public bool CanWallJump { get { return PlayerPrefs.GetInt("CanWallJump", 0) == 1; } }

    //Whether or not the Player is entering the Denial Area Scene for the first time
    public bool FirstTimeInDenial { get { return PlayerPrefs.GetInt("FirstTimeDenial", 1) == 1; } }

    //List of rooms in the scene
    [SerializeField] private List<Room> rooms;

    //Default values to spawn the player at when a New Game is started
    [SerializeField] private float defaultXPos = -16;
    [SerializeField] private float defaultYPos = -1.7f;
    [SerializeField] private string defaultSceneName = "NewGame";
    [SerializeField] private float defaultHP = 125;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //If the player is in the cutscene between Onboarding and Denial Area, transition their stats
        if (SceneManager.GetActiveScene().name == "CutScene")
            TransitionToDenialArea();

        /*PlayerControllerForces.Instance.Data.maxHP = MaxHP;
        PlayerControllerForces.Instance.Data.maxSP = MaxSP;

        PlayerControllerForces.Instance.Data.canDoubleJump = CanDoubleJump;
        PlayerControllerForces.Instance.Data.canDash = CanDash;
        PlayerControllerForces.Instance.Data.canWallJump = CanWallJump;*/
        //PlayerControllerForces.Instance.Data.damageMultiplier = MaxHP;
    }

    //Returns whether a spirit is collected or not (for spawning them in The Drift vs. Equilibrium)
    public bool SpiritCollected(Spirit spirit)
    {
        //Debug.Log(spiritToSpawn.spiritID.ToString());
        if (PlayerPrefs.GetString(spirit.spiritID.ToString(), "Uncollected") != "Uncollected")
        {
            Debug.Log("Spirit Collected");
            return true;
        }

        else
            return false;
    }

    //Return the state of the specified spirit
    public Spirit.SpiritState GetSpiritState(Spirit spirit)
    {
        Enum.TryParse<Spirit.SpiritState>(PlayerPrefs.GetString(spirit.spiritID.ToString()), true, out Spirit.SpiritState spiritState);
        return spiritState;
    }

    //Updates a Spirit's state (i.e. collecting or talking to the spirit)
    public void UpdateSpiritState(Spirit spirit)
    {
        if (spirit.spiritState != Spirit.SpiritState.Idle)
        {
            spirit.spiritState++;

            //Show save icon when spirit is collected
            if (spirit.spiritState == Spirit.SpiritState.Collected)
                StartCoroutine(UIManager.Instance.ShowSaveIcon(2));
        }

        PlayerPrefs.SetString(spirit.spiritID.ToString(), spirit.spiritState.ToString());
    }

    //Saves the user's location information when they use a Save Point
    public void SaveGame(SavePoint saveLocation)
    {
        PlayerPrefs.SetFloat("XSpawnPos", saveLocation.position.x);
        PlayerPrefs.SetFloat("YSpawnPos", saveLocation.position.y);
        PlayerPrefs.SetFloat("RoomIndex", saveLocation.roomIndex);
        PlayerPrefs.SetInt("FirstTimeDenial", 0);
        PlayerPrefs.SetString("SceneSave", SceneManager.GetActiveScene().name);

        StartCoroutine(UIManager.Instance.ShowSaveIcon(2));
    }

    //Toggles whether the user is spawning for the first time or not
    public void ToggleFirstSpawn(bool toggle)
    {
        if (toggle)
            PlayerPrefs.SetInt("FirstSpawn", 1);

        else
            PlayerPrefs.SetInt("FirstSpawn", 0);
    }

    //Reset the save data to the default values (i.e. player starts a new game)
    public void ResetSaveData()
    {
        //Reset scene and location data
        PlayerPrefs.SetString("SceneSave", defaultSceneName);
        PlayerPrefs.SetFloat("XSpawnPos", defaultXPos);
        PlayerPrefs.SetFloat("YSpawnPos", defaultYPos);
        //PlayerPrefs.SetInt("RoomIndex", 0);
        
        //Reset Player Stats
        PlayerPrefs.SetFloat("MaxHP", defaultHP);
        PlayerPrefs.SetFloat("MaxSP", 0);
        PlayerPrefs.SetFloat("DamageMultiplier", 1);

        //Reset Player Abilities
        PlayerPrefs.SetInt("CanDoubleJump", 0);
        PlayerPrefs.SetInt("CanWallJump", 0);
        PlayerPrefs.SetInt("CanDash", 0);

        //Reset Spirit Data
        PlayerPrefs.SetString("Spirit1", "Uncollected");
        PlayerPrefs.SetString("Spirit2", "Uncollected");
        PlayerPrefs.SetString("Spirit3", "Uncollected");

        //Clear Onboarding Map Data
        for (int i = 1; i < 5; i++)
        {
            PlayerPrefs.SetInt("OnboardingLevelRoom" + i, 0);
        } 
    }

    //Transition between Onboarding Level and Denial Area
    private void TransitionToDenialArea()
    {
        //Set first time in Denial (so player starts at Low HP)
        PlayerPrefs.SetInt("FirstTimeDenial", 1);

        //Auto save the Denial Level (i.e. if the player quits after the cutscene they will load into the denial area instead of onboarding
        PlayerPrefs.SetString("SceneSave", "ArenaPlaytestLevel2");

        //Reduce Player Stats and Remove Abilities
        PlayerPrefs.SetFloat("MaxHP", 50);
        PlayerPrefs.SetInt("CanDoubleJump", 0);
        PlayerPrefs.SetInt("CanWallJump", 0);
        PlayerPrefs.SetInt("CanDash", 0);
    }

    public void SetRoomExplored(int roomIndex)
    {
        PlayerPrefs.SetInt("OnboardingLevelRoom" + roomIndex, 1);
        UIManager.Instance.UpdateMapUI();
    }

    public List<bool> AreaRoomsLoaded()
    {
        List<bool> result = new List<bool>();
        foreach (Room room in rooms)
        {
            if (PlayerPrefs.GetInt("OnboardingLevelRoom" + room.roomIndex) == 1)
            {
                result.Add(true);
            }
        }

        return result;
    }
}
