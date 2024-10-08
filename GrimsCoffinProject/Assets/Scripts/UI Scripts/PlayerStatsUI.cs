using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerController player;

    public RectTransform healthBar;
    public Image healthBarFill;
    public RectTransform spiritBar;
    public Image spiritBarFill;

    // Update is called once per frame
    void Update()
    {
        healthBar.sizeDelta = new Vector2(player.maxHP * 7, 35);
        healthBarFill.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(player.maxHP * 7, 35);
        spiritBar.sizeDelta = new Vector2(player.maxSP * 3, 35);
        spiritBarFill.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(player.maxSP * 3, 35);

        healthBarFill.fillAmount = player.currentHP / player.maxHP;
        spiritBarFill.fillAmount = player.currentSP / player.maxSP;
    }
}
