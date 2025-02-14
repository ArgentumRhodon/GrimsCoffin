using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUnlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unlockText;
    [SerializeField] private float lifetime;

    public string unlockMessage;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = lifetime;
        unlockText.text = unlockMessage;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        this.GetComponent<Image>().fillAmount = timer / lifetime;

        if (timer <= 0)
            Destroy(gameObject);
    }
}
