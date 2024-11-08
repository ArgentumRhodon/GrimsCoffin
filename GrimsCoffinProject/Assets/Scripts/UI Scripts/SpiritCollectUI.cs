using UnityEngine;
using UnityEngine.UI; 
using TMPro; // 
using System.Collections;


public class SpiritCollectUI : MonoBehaviour
{
    public Transform player; 
    public RectTransform spiritCollectedText; 
    public Vector3 offset; 
   
    void Start()
    {
        spiritCollectedText.gameObject.SetActive(false);
    }


    void Update()
    {
        if (spiritCollectedText.gameObject.activeSelf)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position + offset);
            spiritCollectedText.position = screenPos;
        }
    }

    public void ShowSpiritCollectedText()
    {
        spiritCollectedText.gameObject.SetActive(true);
        StartCoroutine(HideTextAfterDelay(1f)); 
    }

    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spiritCollectedText.gameObject.SetActive(false);
    }
}
