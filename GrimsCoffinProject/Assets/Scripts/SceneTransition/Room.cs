using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private bool hasPlayer;
    public bool RoomLive;
    [SerializeField] private GameObject Border;
    [SerializeField] private CinemachineVirtualCamera VCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!hasPlayer)
            this.gameObject.SetActive(false);
        if (hasPlayer && this.GetComponent<EnemyManager>() != null)
        {
            this.GetComponent<EnemyManager>().SpawnEnemies();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RoomLive)
        {
            Changeborder(Border);
        }

    }

    private void Changeborder(GameObject border)
    {
            CinemachineConfiner VConfiner = VCamera.GetComponent<CinemachineConfiner>();
            VConfiner.m_BoundingShape2D = border.GetComponent<PolygonCollider2D>();
    }
    public void SetRoomLiveStatus(bool isActive)
    {
        RoomLive = isActive;
    }
}
