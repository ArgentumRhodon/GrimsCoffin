using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerControllerForces player;

    public RectTransform healthBar;
    public Image healthBarFill;
    public RectTransform spiritBar;
    public Image spiritBarFill;

    [SerializeField] private GameObject healthCollectablePrefab;
    [SerializeField] private GameObject healthCollecatbleList;

    private void Start()
    {
        for (int i = 0; i < PersistentDataManager.Instance.HealthCollectablesHeld; i++)
        {
            AddHealthCollectable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.sizeDelta = new Vector2(player.Data.maxHP * 7, 35);
        healthBarFill.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(player.Data.maxHP * 7, 35);
        spiritBar.sizeDelta = new Vector2(player.Data.maxSP * 3, 35);
        spiritBarFill.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(player.Data.maxSP * 3, 35);

        healthBarFill.fillAmount = player.currentHP / player.Data.maxHP;
        spiritBarFill.fillAmount = player.currentSP / player.Data.maxSP;

        UIManager.Instance.LowHealthVignette(LowHealth());
    }

    private bool LowHealth()
    {
        return (player.currentHP / player.Data.maxHP) < .25f;
    }

    public void AddHealthCollectable()
    {
        Instantiate(healthCollectablePrefab, healthCollecatbleList.transform);
    }

    public void RemoveHealthCollectables()
    {
        for (int i = 0; i < 3; i++)
        {
            Destroy(healthCollecatbleList.transform.GetChild(i).gameObject);
        }
    }
}
