using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    public void LoadNextScene()
    {
        if (nextSceneName == "Denial_Level_v1.1")
        {
            GameObject MusicController = GameObject.Find("MusicController_Temp");
            if (MusicController != null)
            {
                Destroy(MusicController);
            }
        }
        SceneManager.LoadScene(nextSceneName);
    }
}
