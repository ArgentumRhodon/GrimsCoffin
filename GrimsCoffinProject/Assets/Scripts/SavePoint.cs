using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : Interactable
{
    public Vector3 position;
    public int roomIndex;

    // Start is called before the first frame update
    void Start()
    {
        position = gameObject.transform.parent.transform.parent.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformInteraction()
    {
        if (Time.timeScale == 0)
            return;

        if (SceneManager.GetActiveScene().name != "Equilibrium")
        {
            PersistentDataManager.Instance.SaveGame(this);
        }

        UIManager.Instance.restPointMenu.ToggleEnterPrompt();
    }
}
