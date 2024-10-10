using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    [SerializeField] private BoxCollider2D promptTrigger;
    [SerializeField] private int iconIndex;
    [SerializeField] private string interactionText;


    // Start is called before the first frame update
    void Start()
    {
        promptTrigger = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController.Instance.interactionPrompt.DisplayPrompt(iconIndex, interactionText);
        }
    }
}
