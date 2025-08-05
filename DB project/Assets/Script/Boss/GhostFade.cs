using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}

