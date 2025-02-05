using Cinemachine;
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
    public int LastSavedRoomIndex { get { return PlayerPrefs.GetInt("RoomIndex", 1); } }

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
    public bool CanScytheThrow { get { return PlayerPrefs.GetInt("CanScytheThrow", 0) == 1; } }
    public bool CanViewMap { get { return PlayerPrefs.GetInt("CanViewMap", 0) == 1; } }

    //Whether or not the Player is entering the Denial Area Scene for the first time
    public bool FirstTimeInDenial { get { return PlayerPrefs.GetInt("FirstTimeDenial", 1) == 1; } }

    //List of rooms in the scene
    [SerializeField] public List<Room> rooms;

    //Default values to spawn the player at when a New Game is started
    [SerializeField] private float defaultXPos = 0;
    [SerializeField] private float defaultYPos = 0;
    [SerializeField] private string defaultSceneName = "OnboardingLevel";
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
            {
                StartCoroutine(UIManager.Instance.ShowSaveIcon(2));
            }

            //Spirit Ability Unlocks
            else if (spirit.spiritState == Spirit.SpiritState.Idle)
            {
                switch (spirit.spiritID)
                {
                    //Unlocks Minimap and Map access
                    case Spirit.SpiritID.MapSpirit:
                        PlayerPrefs.SetInt("CanViewMap", 1);
                        break;

                    //Unlocks Dash
                    case Spirit.SpiritID.DashSpirit:
                        PlayerControllerForces.Instance.Data.canDash = true;
                        PlayerPrefs.SetInt("CanDash", 1);
                        break;

                    //Unlocks Scythe Throw and Spirit Power
                    case Spirit.SpiritID.ScytheThrowSpirit:
                        PlayerControllerForces.Instance.Data.canScytheThrow = true;
                        PlayerControllerForces.Instance.Data.maxSP = 50;
                        PlayerControllerForces.Instance.currentSP = PlayerControllerForces.Instance.Data.maxSP;
                        PlayerPrefs.SetInt("CanScytheThrow", 1);
                        PlayerPrefs.SetFloat("MaxSP", 50);
                        break;

                    //Unlocks Health Upgrades and gives one for free
                    case Spirit.SpiritID.HealthSpirit:
                        PlayerControllerForces.Instance.Data.maxHP += 15;
                        PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP; 
                        PlayerPrefs.SetFloat("MaxHP", PlayerControllerForces.Instance.Data.maxHP);
                        break;
                }
            }
                
        }
        PlayerPrefs.SetString(spirit.spiritID.ToString(), spirit.spiritState.ToString());
    }

    //Saves the user's location information when they use a Save Point
    public void SaveGame(SavePoint saveLocation)
    {
        PlayerPrefs.SetFloat("XSpawnPos", saveLocation.position.x);
        PlayerPrefs.SetFloat("YSpawnPos", saveLocation.position.y);
        PlayerPrefs.SetInt("RoomIndex", saveLocation.roomIndex);
        PlayerPrefs.SetInt("FirstTimeDenial", 0);
        PlayerPrefs.SetString("SceneSave", SceneManager.GetActiveScene().name);

        StartCoroutine(UIManager.Instance.ShowSaveIcon(2));
    }

    //Load the last saved room the player was in
    public void LoadRoom()
    {
        foreach (Room room in rooms)
        {
            if (LastSavedRoomIndex == room.roomIndex)
            {
                room.hasPlayer = true;
                room.RoomLive = true;
                room.gameObject.SetActive(true);

                if (room.GetComponent<EnemyManager>() != null)
                    //room.GetComponent<EnemyManager>().SpawnEnemies();

                this.GetComponent<CameraManager>().Vcam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = room.GetComponent<PolygonCollider2D>();
            }

            else
            {
                room.hasPlayer = false;
                room.RoomLive = false;
            }
        }
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
        PlayerPrefs.SetInt("RoomIndex", 1);
        
        //Reset Player Stats
        PlayerPrefs.SetFloat("MaxHP", defaultHP);
        PlayerPrefs.SetFloat("MaxSP", 0);
        PlayerPrefs.SetFloat("DamageMultiplier", 1);

        //Reset Player Abilities
        PlayerPrefs.SetInt("CanDoubleJump", 0);
        PlayerPrefs.SetInt("CanWallJump", 0);
        PlayerPrefs.SetInt("CanDash", 0);
        PlayerPrefs.SetInt("CanViewMap", 0);

        //Reset Spirit Data
        PlayerPrefs.SetString("MapSpirit", "Uncollected");
        PlayerPrefs.SetString("DashSpirit", "Uncollected");
        PlayerPrefs.SetString("ScytheThrowSpirit", "Uncollected");
        PlayerPrefs.SetString("HealthSpirit", "Uncollected");

        //Clear Onboarding Map Data
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.SetInt("LevelRoom" + i, 0);
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
        PlayerPrefs.SetInt("LevelRoom" + roomIndex, 1);

        UIManager.Instance.UpdateMapUI();
    }

    public List<bool> AreaRoomsLoaded()
    {
        List<bool> result = new List<bool>();
        foreach (Room room in rooms)
        {
            if (PlayerPrefs.GetInt("LevelRoom" + room.roomIndex) == 1)
            {
                result.Add(true);
            }
            else
            {
                result.Add(false);
            }
        }

        return result;
    }
}
