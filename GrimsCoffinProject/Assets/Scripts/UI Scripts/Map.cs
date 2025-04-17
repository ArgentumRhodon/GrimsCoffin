using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Map : MonoBehaviour
{
    [SerializeField] private List<GameObject> promptTrays;
    [SerializeField] public GameObject fullMapUI;
    [SerializeField] private Camera fullMapCamera;
    [SerializeField] private GameObject mapKey;
    [SerializeField] private TextMeshProUGUI exploredPercentage;
    [SerializeField] private TextMeshProUGUI spiritProgress;

    private bool mapKeyActive = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Show proper prompt tray based on input used
        switch (PersistentDataManager.Instance.ControlScheme)
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

            default:
                promptTrays[0].SetActive(false);
                promptTrays[1].SetActive(false);
                promptTrays[2].SetActive(true);
                break;
        }

        exploredPercentage.text = ReturnExploredPercentage() + "% Explored";
        spiritProgress.text = ReturnSpiritsCollected() + "/4 Spirits";

        if (ReturnExploredPercentage() == "100")
            exploredPercentage.color = Color.green;

        if (ReturnSpiritsCollected() == "4")
            spiritProgress.color = Color.green;
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
        if (!this.gameObject.activeInHierarchy)
            return;

        mapKeyActive = !mapKeyActive;
        mapKey.SetActive(mapKeyActive);
    }

    public string ReturnExploredPercentage()
    {
        float roomsExplored = 0;
        float totalRooms = PersistentDataManager.Instance.rooms.Count;

        foreach (bool explored in PersistentDataManager.Instance.AreaRoomsLoaded())
        {
            if (explored)
                roomsExplored++;
        }

        return ((roomsExplored / totalRooms) * 100).ToString("F0");
    }

    public string ReturnSpiritsCollected()
    {
        Spirit[] spirits = FindObjectsOfType<Spirit>();

        int spiritsCollected = 4 - spirits.Length;

        return spiritsCollected.ToString();
    }
}
