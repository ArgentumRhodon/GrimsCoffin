using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FakeWall : MonoBehaviour
{
    [SerializeField]
    private Tilemap fWall;

    // Start is called before the first frame update
    void Start()
    {
        fWall = transform.parent.GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FadeIn(float duration)
    {
        Color start = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
        Color target = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 0f);

        yield return Fade(start, target, duration);

    }
    IEnumerator FadeOut(float duration)
    {
        Color target = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
        Color start = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 0f);

        yield return Fade(start, target, duration);
    }

    IEnumerator Fade(Color start, Color target, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercentage = 0;

        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            fWall.color = Color.Lerp(start, target, elapsedPercentage);

            yield return null;

            elapsedTime += Time.deltaTime;
        }
    }

    IEnumerator WallFadeOut()
    {
        //Color start = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
        //Color target = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
        //yield return Fade(start, target, 0.5f);
        yield return FadeIn(0.5f);
    }

    IEnumerator WallFadeIn()
    {
        //Color start = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
        //Color target = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
        //yield return Fade(start, target, 0.5f);
        yield return FadeOut(0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            //fWall.color = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 0f);
            StartCoroutine(WallFadeOut());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            //fWall.color = new Color(fWall.color.r, fWall.color.g, fWall.color.b, 1f);
            StartCoroutine(WallFadeIn());
    }
}
