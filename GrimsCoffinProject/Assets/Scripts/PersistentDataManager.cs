using Cinemachine;
using Pathfinding.Ionic.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; private set; }

    //Game Progress Flags
    public Vector2 SpawnPoint { get { return new Vector2(PlayerPrefs.GetFloat("XSpawnPos", defaultXPos), PlayerPrefs.GetFloat("YSpawnPos", defaultYPos)); } }
    public string LastSavedScene { get { return PlayerPrefs.GetString("SceneSave", defaultSceneName); } }
    public int LastSavedRoomIndex { get { return PlayerPrefs.GetInt("RoomIndex", -1); } }
    public bool FirstSpawn { get { return PlayerPrefs.GetInt("FirstSpawn", 0) == 1; } }
    public bool FirstTimeInDenial { get { return PlayerPrefs.GetInt("FirstTimeDenial", 1) == 1; } }

    //Player Stat Values
    public float MaxHP { get { return PlayerPrefs.GetFloat("MaxHP", defaultHP); } }
    public float MaxSP { get { return PlayerPrefs.GetFloat("MaxSP", 0); } }
    public float DamageMultiplier { get { return PlayerPrefs.GetFloat("DamageMultiplier", 1); } }

    //Player Ability Flags
    public bool CanDoubleJump { get { return PlayerPrefs.GetInt("CanDoubleJump", 0) == 1; } }
    public bool CanDash { get { return PlayerPrefs.GetInt("CanDash", 1) == 1; } }
    public bool CanWallJump { get { return PlayerPrefs.GetInt("CanWallJump", 1) == 1; } }
    public bool CanScytheThrow { get { return PlayerPrefs.GetInt("CanScytheThrow", 0) == 1; } }
    public bool CanViewMap { get { return PlayerPrefs.GetInt("CanViewMap", 0) == 1; } }
    public int MapBought { get { return PlayerPrefs.GetInt("MapBought"); } }
    public bool CanUpAttack {  get { return PlayerPrefs.GetInt("CanUpAttack", 1) == 1; } }
    public bool CanDownAttack { get { return PlayerPrefs.GetInt("CanDownAttack", 1) == 1; } }

    //Resources
    public int HealthCollectablesHeld { get { return PlayerPrefs.GetInt("HealthCollectablesHeld", 0); } }
    public float EnemyCurrency { get { return PlayerPrefs.GetFloat("EnemyCurrency"); } }

    public string ControlScheme { get { return PlayerPrefs.GetString("ControlScheme"); } }

    //List of pesistently tracked objects in the scene
    [SerializeField] public List<Room> rooms;
    [SerializeField] public List<HealthUpgrade> healthUpgrades;
    [SerializeField] public List<ScytheThrowRope> scytheThrowPlatforms;
    [SerializeField] public List<ArenaManager> arenas;
    [SerializeField] public List<SavePoint> restPoints;

    //Default values to spawn the player at when a New Game is started
    [SerializeField] private float defaultXPos = -16;
    [SerializeField] private float defaultYPos = -2f;
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
        if (SceneManager.GetActiveScene().name == "Transition Cutscene 1 Autoplay")
            TransitionToDenialArea();

        for (int i = 0; i < scytheThrowPlatforms.Count; i++)
        {
            if (PlayerPrefs.GetInt("ScythePlatform" + scytheThrowPlatforms[i].ropeIndex) == 1)
                scytheThrowPlatforms[i].TakeDamage(1);
        }

        for (int i = 0; i < arenas.Count; i++)
        {
            if (PlayerPrefs.GetInt("Arena" + arenas[i].arenaIndex) == 1)
                arenas[i].arenaCleared = true;
        }
    }

    private void Update()
    {
        if (UIManager.Instance != null)
            PlayerPrefs.SetString("ControlScheme", UIManager.Instance.playerInput.currentControlScheme.ToString());

        else
            PlayerPrefs.SetString("ControlScheme", GameObject.Find("PlayerControls").GetComponent<PlayerInput>().currentControlScheme.ToString());
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

            //Show save icon when spirit is collected and unlock ability
            if (spirit.spiritState == Spirit.SpiritState.Collected)
            {
                StartCoroutine(UIManager.Instance.ShowSaveIcon(2));
                switch (spirit.spiritID)
                {
                    //Unlocks Minimap and Map access
                    case Spirit.SpiritID.MapSpirit:
                        PlayerPrefs.SetInt("CanViewMap", 1);
                        PlayerControllerForces.Instance.Data.canViewMap = true;
                        UIManager.Instance.ShowAbilityUnlock("Map Unlocked", AbilityName.Map, true);
                        UIManager.Instance.ToggleMap();
                        break;

                    //Unlocks Dash
                    case Spirit.SpiritID.DashSpirit:
                        PlayerControllerForces.Instance.Data.canDash = true;
                        PlayerPrefs.SetInt("CanDash", 1);
                        UIManager.Instance.ShowAbilityUnlock("Dash Unlocked", AbilityName.Dash);
                        break;

                    //Unlocks Scythe Throw and Spirit Power
                    case Spirit.SpiritID.ScytheThrowSpirit:
                        PlayerControllerForces.Instance.Data.canScytheThrow = true;
                        //PlayerControllerForces.Instance.Data.maxSP = 50;
                        //PlayerControllerForces.Instance.currentSP = PlayerControllerForces.Instance.Data.maxSP;
                        PlayerPrefs.SetInt("CanScytheThrow", 1);
                        UIManager.Instance.ShowAbilityUnlock("Scythe Throw Unlocked", AbilityName.ScytheThrow);
                        //PlayerPrefs.SetFloat("MaxSP", 50);
                        break;
                    case Spirit.SpiritID.CombatSpirit:
                        PlayerControllerForces.Instance.Data.damageMultiplier = 1.5f;
                        PlayerPrefs.SetFloat("DamageMultiplier", 1.5f);
                        UIManager.Instance.ShowAbilityUnlock("Damage Increased", AbilityName.NoAbility);
                        break;
                }
            }

            //Unlocks Health Upgrades and gives one for free
            else if (spirit.spiritState == Spirit.SpiritState.Unlocked && spirit.spiritID == Spirit.SpiritID.HealthSpirit)
            {
                PlayerControllerForces.Instance.Data.maxHP += 10;
                PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP;
                PlayerPrefs.SetFloat("MaxHP", PlayerControllerForces.Instance.Data.maxHP);
                UIManager.Instance.ShowAbilityUnlock("Max Health Increased", AbilityName.NoAbility);
            }

            else if (spirit.spiritState == Spirit.SpiritState.Idle && spirit.spiritID == Spirit.SpiritID.MapSpirit)
            {
                PlayerPrefs.SetInt("MapBought", 1);
                UpdateEnemyCurrency(-spirit.upgradeCost, false);
                UIManager.Instance.ShowAbilityUnlock("Map Purchased", AbilityName.NoAbility);
            }

            else if (spirit.spiritState == Spirit.SpiritState.Idle && spirit.spiritID == Spirit.SpiritID.CombatSpirit)
            {
                if (!CanUpAttack)
                {
                    PlayerControllerForces.Instance.Data.canAUpAttack = true;
                    PlayerControllerForces.Instance.Data.canGUpAttack = true;
                    PlayerPrefs.SetInt("CanUpAttack", 1);
                    UpdateEnemyCurrency(-spirit.upgradeCost, false);
                    UIManager.Instance.ShowAbilityUnlock("Up Attack Purchased", AbilityName.NoAbility);

                    spirit.spiritState = Spirit.SpiritState.Unlocked;
                    spirit.upgradeCost = 1250;
                }

                else
                {
                    PlayerControllerForces.Instance.Data.canADownAttack = true;
                    PlayerControllerForces.Instance.Data.canGDownAttack = true;
                    PlayerPrefs.SetInt("CanDownAttack", 1);
                    UpdateEnemyCurrency(-spirit.upgradeCost, false);
                    UIManager.Instance.ShowAbilityUnlock("Down Attack Purchased", AbilityName.NoAbility);
                }
            }

            //Trade in health collectables for health upgrade
            else if (spirit.spiritID == Spirit.SpiritID.HealthSpirit && spirit.spiritState == Spirit.SpiritState.Idle)
            {
                PlayerControllerForces.Instance.Data.maxHP += 10;
                PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP;
                PlayerPrefs.SetFloat("MaxHP", PlayerControllerForces.Instance.Data.maxHP);

                int collectablesHeld = HealthCollectablesHeld;

                collectablesHeld -= (int)spirit.upgradeCost;
                Mathf.Clamp(collectablesHeld, 0, 100);

                PlayerPrefs.SetInt("HealthCollectablesHeld", collectablesHeld);
                UIManager.Instance.ShowAbilityUnlock("Max Health Increased", AbilityName.NoAbility);
                UIManager.Instance.RemoveHealthCollectables();

                if (PlayerControllerForces.Instance.Data.maxHP < 80)
                    spirit.spiritState = Spirit.SpiritState.Unlocked;
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
            if (room.RoomLive)
                room.gameObject.SetActive(false);

            if (LastSavedRoomIndex == room.roomIndex)
            {
                room.hasPlayer = true;
                room.RoomLive = true;
                room.gameObject.SetActive(true);

                if (room.GetComponent<EnemyManager>() != null)
                    room.GetComponent<EnemyManager>().SpawnEnemies();

                this.GetComponent<CameraManager>().Vcam.GetComponent<CinemachineConfiner>().m_BoundingShape2D = room.GetComponent<PolygonCollider2D>();

                foreach (SavePoint restPoint in restPoints)
                {
                    if (restPoint.roomIndex == room.roomIndex)
                        restPoint.isActive = true;

                    else
                        restPoint.isActive = false;
                }
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
        PlayerPrefs.SetInt("RoomIndex", -1);
        
        //Reset Player Stats
        PlayerPrefs.SetFloat("MaxHP", defaultHP);
        PlayerPrefs.SetFloat("MaxSP", 0);
        PlayerPrefs.SetFloat("DamageMultiplier", 3);

        //Reset Player Abilities
        PlayerPrefs.SetInt("CanDoubleJump", 1);
        PlayerPrefs.SetInt("CanWallJump", 1);
        PlayerPrefs.SetInt("CanDash", 1);
        PlayerPrefs.SetInt("CanViewMap", 0);
        PlayerPrefs.SetInt("CanScytheThrow", 0);
        PlayerPrefs.SetInt("CanUpAttack", 1);
        PlayerPrefs.SetInt("CanDownAttack", 1);

        //Reset Spirit Data
        PlayerPrefs.SetString("MapSpirit", "Uncollected");
        PlayerPrefs.SetString("DashSpirit", "Uncollected");
        PlayerPrefs.SetString("ScytheThrowSpirit", "Uncollected");
        PlayerPrefs.SetString("HealthSpirit", "Uncollected");
        PlayerPrefs.SetString("CombatSpirit", "Uncollected");
        PlayerPrefs.SetString("Onboarding1", "Uncollected");
        PlayerPrefs.SetString("Onboarding2", "Uncollected");
        PlayerPrefs.SetString("Onboarding3", "Uncollected");


        PlayerPrefs.SetInt("HealthCollectablesHeld", 0);
        PlayerPrefs.SetFloat("EnemyCurrency", 0);
        PlayerPrefs.SetInt("MapBought", 0);

        //Clear Onboarding Map Data
        for (int i = 0; i < 30; i++)
        {
            PlayerPrefs.SetInt("LevelRoom" + i, 0);
        } 

        //Reset health collectables
        for (int i = 0; i < 25; i++)
        {
            PlayerPrefs.SetInt("HealthCollectable" + i, 0);
        }

        //Reset Scythe Throw Platforms
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt("ScythePlatform" + i, 0);
        }

        //Reset Arenas
        for (int i = 0; i < 10; i++)
        {
            PlayerPrefs.SetInt("Arena" + i, 0);
        }
    }

    //Transition between Onboarding Level and Denial Area
    private void TransitionToDenialArea()
    {
        //Set first time in Denial (so player starts at Low HP)
        PlayerPrefs.SetInt("FirstTimeDenial", 1);

        //Auto save the Denial Level (i.e. if the player quits after the cutscene they will load into the denial area instead of onboarding
        PlayerPrefs.SetString("SceneSave", "Denial_Level_v1.1");
        PlayerPrefs.SetFloat("XSpawnPos", -17.9f);

        //Reduce Player Stats and Remove Abilities
        PlayerPrefs.SetFloat("MaxHP", 50);
        PlayerPrefs.SetFloat("DamageMultiplier", 1);
        PlayerPrefs.SetInt("CanDoubleJump", 0);
        //PlayerPrefs.SetInt("CanWallJump", 0);
        PlayerPrefs.SetInt("CanUpAttack", 0);
        PlayerPrefs.SetInt("CanDownAttack", 0);
        PlayerPrefs.SetInt("CanDash", 0);
        PlayerPrefs.SetString("HealthSpirit", "Collected");
    }

    /// <summary>
    /// Sets a room to be explored so it is properly displayed on the Map UI
    /// </summary>
    /// <param name="roomIndex">The index of the room that has been explored</param>
    public void SetRoomExplored(int roomIndex)
    {
        PlayerPrefs.SetInt("LevelRoom" + roomIndex, 1);

        UIManager.Instance.UpdateMapUI();
    }

    public void SetOnboardingSpawnPoint(int roomIndex)
    {
        if (SceneManager.GetActiveScene().name == "OnboardingLevel")
        {
            PlayerPrefs.SetFloat("XSpawnPos", PlayerControllerForces.Instance.gameObject.transform.position.x);
            PlayerPrefs.SetFloat("YSpawnPos", PlayerControllerForces.Instance.gameObject.transform.position.y);
            PlayerPrefs.SetInt("RoomIndex", roomIndex);
        }
    }

    /// <summary>
    /// Get all of the rooms that have been explored by the player
    /// </summary>
    /// <returns>Returns a list of bools, with true entries referring to explored rooms and false entries referring to unexplored rooms</returns>
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

    /// <summary>
    /// Get which health collectables the player has obtained
    /// </summary>
    /// <returns>Returns a list of bools showing which health collectables have been obtained and which have not</returns>
    public List<bool> HealthUpgradesCollected()
    {
        List<bool> result = new List<bool>();
        foreach(HealthUpgrade collectable in healthUpgrades)
        {
            if (PlayerPrefs.GetInt("HealthCollectable" + collectable.collectableID) == 1)
                result.Add(true);

            else
                result.Add(false);
        }

        return result;
    }

    /// <summary>
    /// Update how many health collectables the player has held when they pick one up
    /// </summary>
    /// <param name="collectableID">ID value for the health collectable</param>
    public void CollectHealthUpgrade(int collectableID)
    {
        UIManager.Instance.AddHealthCollectable();
        PlayerPrefs.SetInt("HealthCollectablesHeld", HealthCollectablesHeld + 1);
        PlayerPrefs.SetInt("HealthCollectable" + collectableID, 1);
    }

    /// <summary>
    /// Set the flag for the ScythePlatform to be cut down
    /// </summary>
    /// <param name="ropeIndex">Index for the platform ID</param>
    public void CutPlatform(int ropeIndex)
    {
        PlayerPrefs.SetInt("ScythePlatform" + ropeIndex, 1);
    }

    /// <summary>
    /// Set the flag for an arena being cleared
    /// </summary>
    /// <param name="arenaIndex">Index for the arena ID</param>
    public void ClearArena(int arenaIndex)
    {
        PlayerPrefs.SetInt("Arena" + arenaIndex, 1);
    }

    /// <summary>
    /// Update how much currency the player currently has
    /// </summary>
    /// <param name="value">The amount of currency to add/remove</param>
    /// <param name="addingCurrency">Whether or not the currency is being added or subtracted from the player</param>
    public void UpdateEnemyCurrency(float value, bool addingCurrency)
    {
        if (UIManager.Instance.enemyCurrencyUI == null)
            return;

        PlayerPrefs.SetFloat("EnemyCurrency", EnemyCurrency + value);
        UIManager.Instance.UpdateEnemyCurrency(value, addingCurrency);
    }
}
