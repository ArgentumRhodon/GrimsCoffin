using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private bool hasPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (!hasPlayer)
            this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
