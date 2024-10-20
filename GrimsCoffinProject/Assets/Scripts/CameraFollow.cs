using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Camera camera;

    private bool clampX;
    private bool clampY;

    private float yClamp;
    private float xClamp;

    [SerializeField]
    private float roomXMin;
    [SerializeField]
    private float roomXMax;
    [SerializeField]
    private float roomYMin;
    [SerializeField]
    private float roomYMax;

    private float transitionSpeed;
    private float camSize;
    private bool isShifting;
    private Vector3 endShift;
    private float shiftPercent;

    // Start is called before the first frame update
    void Start()
    {
        transitionSpeed = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (!isShifting)
        {
            pos = Vector3.Lerp(transform.position, PlayerController.Instance.transform.position + offset, followSpeed);
            if (clampX)
                pos.x = xClamp;
            if (clampY)
                pos.y = yClamp;

            transform.position = pos;

            pos = Vector3.Lerp(transform.position, CheckBorders(pos), 0.75f);
            transform.position = pos;
        }
        else
        {
            pos = Vector3.Lerp(transform.position, endShift, transitionSpeed);
            transform.position = pos;

            //if (camera.orthographicSize < camSize)
            //{
            //    while(camera.orthographicSize < camSize)
            //        camera.orthographicSize += 0.01f;
            //}
            //else if (camera.orthographicSize > camSize)
            //{
            //    while (camera.orthographicSize > camSize)
            //        camera.orthographicSize -= 0.01f;
            //}

            transitionSpeed += 0.01f;

            if(transform.position == endShift)
            {
                isShifting = false;
                Debug.Log("end shift");
                transitionSpeed = 0.01f;
            }
            
            
        }
    }

    public void ClampX(float x)
    {
        clampX = true;
        xClamp = x;
    }

    public void ClampY(float y)
    {
        clampY = true;
        yClamp = y;
    }

    public void StopClampX()
    {
        clampX = false;
    }
    public void StopClampY()
    {
        clampY = false;
    }

    public void SizeChange(float size)
    {
        camSize = size;
    }

    public void SetBorders(float x1, float x2, float y1, float y2)
    {
        roomXMin = x1;
        roomXMax = x2;
        roomYMin = y1;
        roomYMax = y2;
        endShift = CheckBorders(transform.position);

        isShifting = true;
    }

    private Vector3 CheckBorders(Vector3 pos)
    {
        Vector2 bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
        Vector2 topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        float halfWidth = (topRight.x - bottomLeft.x) / 2;
        float halfHeight = (topRight.y - bottomLeft.y) / 2;
        if (!clampX)
        {
            if (bottomLeft.x <= roomXMin)
                pos.x = roomXMin + halfWidth;
            if (topRight.x >= roomXMax)
                pos.x = roomXMax - halfWidth;
        }
        if (!clampY)
        {
            if (bottomLeft.y <= roomYMin)
                pos.y = roomYMin + halfHeight;
            if (topRight.y >= roomYMax)
                pos.y = roomYMax - halfHeight;
        }

        return pos;

    }
}
