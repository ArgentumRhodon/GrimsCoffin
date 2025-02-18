using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectOnStart : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToEnable;

    void Start()
    {
        objectToEnable.SetActive(true);
    }
}
