using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;

    public void Update()
    {
        currencyText.text = PersistentDataManager.Instance.EnemyCurrency.ToString();
    }
}
