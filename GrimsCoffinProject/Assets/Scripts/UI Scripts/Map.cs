using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] private List<GameObject> promptTrays;
    [SerializeField] public GameObject fullMapUI;
    [SerializeField] private Camera fullMapCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (UIManager.Instance.playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                promptTrays[0].SetActive(true);
                promptTrays[1].SetActive(false);
                promptTrays[2].SetActive(false);
                break;

            case "Playstation":
                promptTrays[0].SetActive(false);
                promptTrays[1].SetActive(true);
                promptTrays[2].SetActive(false);
                break;

            case "Xbox":
                promptTrays[0].SetActive(false);
                promptTrays[1].SetActive(false);
                promptTrays[2].SetActive(true);
                break;
        }
    }

    public void ZoomMap(bool zoomIn)
    {
        if (fullMapUI == null)
            return;

        if (fullMapUI.activeInHierarchy)
        {
            if (zoomIn && UIManager.Instance.playerInput.currentControlScheme != "Keyboard&Mouse")
                fullMapCamera.orthographicSize -= 1;

            else if (!zoomIn && UIManager.Instance.playerInput.currentControlScheme != "Keyboard&Mouse")
                fullMapCamera.orthographicSize += 1;

            else if (zoomIn && UIManager.Instance.playerInput.currentControlScheme == "Keyboard&Mouse")
                fullMapCamera.orthographicSize -= 10;

            else
                fullMapCamera.orthographicSize += 10;

            fullMapCamera.orthographicSize = Mathf.Clamp(fullMapCamera.orthographicSize, 20, 256);
        }
    }

    public void PanMap(Vector2 input)
    {
        if (fullMapUI == null)
            return;

        if (fullMapUI.activeInHierarchy)
        {
            fullMapCamera.transform.position += new Vector3(input.x, input.y, 0);
        }
    }

    public void ResetMap()
    {
        fullMapCamera.transform.position = new Vector3(PlayerControllerForces.Instance.transform.position.x, PlayerControllerForces.Instance.transform.position.y, -10);
        fullMapCamera.orthographicSize = 35;
    }
}
