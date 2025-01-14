using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; private set; }

    List<Spirit> spirits;

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

    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void SetSpawnLocation()
    {

    }
}
