using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    void OnEnable()
    {
        LoadNextScene();
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
