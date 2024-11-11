using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private bool hasPlayer;
    private bool RoomLive;
    [SerializeField] private GameObject Border;
    [SerializeField] private CinemachineVirtualCamera VCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!hasPlayer)
            this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasPlayer)
            Changeborder(Border);
    }

    private void Changeborder(GameObject border)
    {
        if (!RoomLive) 
        {
            CinemachineConfiner VConfiner = VCamera.GetComponent<CinemachineConfiner>();
            VConfiner.m_BoundingShape2D = border.GetComponent<PolygonCollider2D>();
            RoomLive = true;
        }
    }
}
