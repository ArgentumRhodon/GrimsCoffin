using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    [SerializeField] private BoxCollider2D promptTrigger;

    //Index for control icon, reference the lists within the InteractionPrompt prefab for which index correlates to which icon
    [SerializeField] private int iconIndex;

    //Text for what the interaction being prompted is
    [SerializeField] private string interactionText;

    [SerializeField] private float time;


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
        //Show prompt if player enters the trigger
        if (collision.gameObject.GetComponent<PlayerControllerForces>() != null)
        {
            PlayerControllerForces.Instance.interactionPrompt.DisplayPrompt(iconIndex, interactionText, time);
        }
    }
}
