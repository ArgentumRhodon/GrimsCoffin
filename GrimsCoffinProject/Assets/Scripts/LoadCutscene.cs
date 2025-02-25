using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadCutscene : MonoBehaviour
{
    [SerializeField] private Image screenFade;
    private BoxCollider2D trigger;
    // Start is called before the first frame update
    void Start()
    {
        trigger = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerControllerForces>() != null)
        {
            StartCoroutine(Transition());
        }
    }
    IEnumerator Transition() 
    {
        yield return FadeOut(0.5f);
        LoadNextScene();
    }
    private void LoadNextScene() 
    {
        SceneManager.LoadScene("Scene Transition Cutscene");
    }
    IEnumerator FadeOut(float duration)
    {
        Color target = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 1f);
        Color start = new Color(screenFade.color.r, screenFade.color.g, screenFade.color.b, 0f);

        yield return Fade(start, target, duration);
    }
    IEnumerator Fade(Color start, Color target, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            screenFade.color = Color.Lerp(start, target, elapsedPercentage);

            yield return null;

            elapsedTime += Time.deltaTime;
        }
    }
}
