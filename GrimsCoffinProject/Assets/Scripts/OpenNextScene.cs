using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // This will allow you to assign the next scene in the editor using its name
    [SerializeField] private string nextSceneName;

    // Function to load the next scene
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(WaitAndLoad(0.6f));
        }
        else
        {
            Debug.LogError("Next scene name is not set!");
        }
    }

    IEnumerator WaitAndLoad(float Delay)
    {
        yield return new WaitForSeconds(Delay);
        SceneManager.LoadScene(nextSceneName);
    }
}