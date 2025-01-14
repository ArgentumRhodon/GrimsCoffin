using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : Interactable
{
    [SerializeField] private Vector3 position;
    [SerializeField] private int roomIndex;
    private string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
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

        PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP;
        PlayerControllerForces.Instance.Data.respawnPoint = this.transform.position;
        UIManager.Instance.equilibriumPrompt.ToggleEnterPrompt();
    }
}
