using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyCurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI currencyAddedText;

    private float currencyAdded = 0;

    public void Update()
    {
        currencyText.text = PersistentDataManager.Instance.EnemyCurrency.ToString();
        if (currencyAddedText.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            currencyAddedText.gameObject.SetActive(false);
            currencyAdded = 0;
        }
    }

    public void UpdateCurrency (float value, bool addingCurrency)
    {
        if (currencyAddedText.gameObject.activeInHierarchy)
        {
            currencyAddedText.gameObject.SetActive(false);
        }

        currencyAdded += value;
        currencyAddedText.gameObject.SetActive(true);

        if (addingCurrency)
            currencyAddedText.text = "+" + currencyAdded;
        else
            currencyAddedText.text = "-" + currencyAdded;
    }
}
