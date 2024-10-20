using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShift : MonoBehaviour
{
    [SerializeField]
    private CameraFollow mainCam;

    [SerializeField]
    private float roomXMin;
    [SerializeField]
    private float roomXMax;
    [SerializeField]
    private float roomYMin;
    [SerializeField]
    private float roomYMax;
    [SerializeField]
    private float size;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            mainCam.SetBorders(roomXMin, roomXMax, roomYMin, roomYMax);
            if(size > 0)
            {
                mainCam.SizeChange(size);
            }
        }
    }
}
