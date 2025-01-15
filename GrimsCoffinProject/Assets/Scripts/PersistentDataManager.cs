using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; private set; }

    public Vector2 SpawnPoint { get { return new Vector2(PlayerPrefs.GetFloat("XSpawnPos"), PlayerPrefs.GetFloat("YSpawnPos")); } }
    public string LastSavedScene { get { return PlayerPrefs.GetString("SceneSave"); } }
    public int LastSavedRoomIndex { get { return PlayerPrefs.GetInt("RoomIndex", 0); } }
    public bool FirstSpawn { get { return PlayerPrefs.GetInt("FirstSpawn") == 1; } }

    public float MaxHP { get { return PlayerPrefs.GetFloat("MaxHP"); } }
    public float MaxSP { get { return PlayerPrefs.GetFloat("MaxSP"); } }
    public float DamageMultiplier { get { return PlayerPrefs.GetFloat("DamageMultiplier"); } }

    public bool CanDoubleJump { get { return PlayerPrefs.GetInt("CanDoubleJump") == 1; } }
    public bool CanDash { get { return PlayerPrefs.GetInt("CanDash") == 1; } }
    public bool CanWallJump { get { return PlayerPrefs.GetInt("CanWallJump") == 1; } }

    [SerializeField] private List<Room> rooms;


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
        /*PlayerControllerForces.Instance.Data.maxHP = MaxHP;
        PlayerControllerForces.Instance.Data.maxSP = MaxSP;

        PlayerControllerForces.Instance.Data.canDoubleJump = CanDoubleJump;
        PlayerControllerForces.Instance.Data.canDash = CanDash;
        PlayerControllerForces.Instance.Data.canWallJump = CanWallJump;*/
        //PlayerControllerForces.Instance.Data.damageMultiplier = MaxHP;
    }

    public bool SpiritCollected(Spirit spirit)
    {
        //Debug.Log(spiritToSpawn.spiritID.ToString());
        if (PlayerPrefs.GetString(spirit.spiritID.ToString()) != "Uncollected")
        {
            Debug.Log("Spirit Collected");
            return true;
        }

        else
            return false;
    }
    public void UpdateSpiritState(Spirit spirit)
    {
        if (spirit.spiritState != Spirit.SpiritState.Idle)
        {
            spirit.spiritState++;
        }

        PlayerPrefs.SetString(spirit.spiritID.ToString(), spirit.spiritState.ToString());
    }

    public void SaveGame(SavePoint saveLocation)
    {
        PlayerPrefs.SetFloat("XSpawnPos", saveLocation.position.x);
        PlayerPrefs.SetFloat("YSpawnPos", saveLocation.position.y);
        PlayerPrefs.SetFloat("RoomIndex", saveLocation.roomIndex);
        PlayerPrefs.SetString("SceneSave", SceneManager.GetActiveScene().name);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(LastSavedScene);
    }

    public void ToggleFirstSpawn(bool toggle)
    {
        if (toggle)
            PlayerPrefs.SetInt("FirstSpawn", 1);

        else
            PlayerPrefs.SetInt("FirstSpawn", 0);
    }

    public void ResetSaveData()
    {
        //PlayerPrefs.SetString("SceneSave", "IntroScene");
        //PlayerPrefs.SetFloat("XSpawnPos", 0);
        //PlayerPrefs.SetFloat("YSpawnPos", 0);
        //PlayerPrefs.SetInt("RoomIndex", 0);

        PlayerPrefs.SetFloat("MaxHP", 125);
        PlayerPrefs.SetFloat("MaxSP", 0);
        PlayerPrefs.SetFloat("DamageMultiplier", 1);

        PlayerPrefs.SetString("DashSpirit", "Uncollected");
        PlayerPrefs.SetString("ScytheThrowSpirit", "Uncollected");
        PlayerPrefs.SetString("HealthSpirit", "Uncollected");
    }

    public void SaveRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].hasPlayer)
                PlayerPrefs.SetInt("LastSavedRoom", i);
        }
    }

    public void LoadRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i == LastSavedRoomIndex)
            {
                rooms[i].hasPlayer = true;
            }
        }
    }
}
