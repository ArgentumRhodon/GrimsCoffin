using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] private List<GameObject> promptTrays;
    [SerializeField] public GameObject fullMapUI;
    [SerializeField] private Camera fullMapCamera;
    [SerializeField] private GameObject mapKey;

    private bool mapKeyActive;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Show proper prompt tray based on input used
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

    //Zoom the map in/out
    public void ZoomMap(bool zoomIn)
    {
        if (fullMapUI == null)
            return;

        if (fullMapUI.activeInHierarchy)
        {
            //Zooming in for controller
            if (zoomIn && UIManager.Instance.playerInput.currentControlScheme != "Keyboard&Mouse")
                fullMapCamera.orthographicSize -= 1;

            //Zooming out for controller
            else if (!zoomIn && UIManager.Instance.playerInput.currentControlScheme != "Keyboard&Mouse")
                fullMapCamera.orthographicSize += 1;

            //Zooming in for Mouse & Keyboard
            else if (zoomIn && UIManager.Instance.playerInput.currentControlScheme == "Keyboard&Mouse")
                fullMapCamera.orthographicSize -= 10;

            //Zooming out for Mouse & Keyboard
            else
                fullMapCamera.orthographicSize += 10;

            fullMapCamera.orthographicSize = Mathf.Clamp(fullMapCamera.orthographicSize, 20, 256);
        }
    }

    //Pan the map screen
    public void PanMap(Vector2 input, bool drag)
    {
        if (fullMapUI == null)
            return;

        if (fullMapUI.activeInHierarchy)
        {
            //Update transform based on input
            if (!drag)
                fullMapCamera.transform.position += new Vector3(input.x, input.y, 0) * Time.unscaledDeltaTime * 70;

            else
                fullMapCamera.transform.position += new Vector3(input.x, input.y, 0) * Time.unscaledDeltaTime * (fullMapCamera.orthographicSize/2);

        }
    }

    public void ResetMap()
    {
        fullMapCamera.transform.position = new Vector3(PlayerControllerForces.Instance.transform.position.x, PlayerControllerForces.Instance.transform.position.y, -10);
        fullMapCamera.orthographicSize = 35;
    }

    public void ToggleMapKey()
    {
        mapKeyActive = !mapKeyActive;
        mapKey.SetActive(mapKeyActive);
    }
}
