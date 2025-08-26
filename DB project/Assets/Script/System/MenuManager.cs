using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressAnyKeyText;

    [Header("Settings")]
    public float blinkDuration = 1f; // 깜박임 속도
    private bool isProcessing = false;

    private void Start()
    {
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(true);
            StartCoroutine(BlinkText());
        }
    }

    private void Update()
    {
        if (isProcessing) return;

        // Z 또는 Enter 키 입력 시
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SelectAndLoadMain());
        }
    }

    private IEnumerator BlinkText()
    {
        while (!isProcessing)
        {
            if (pressAnyKeyText == null) yield break;

            // Fade Out
            yield return StartCoroutine(FadeText(1f, 0f, blinkDuration / 2f));
            // Fade In
            yield return StartCoroutine(FadeText(0f, 1f, blinkDuration / 2f));
        }
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        if (pressAnyKeyText == null) yield break;

        float time = 0f;
        Color color = pressAnyKeyText.color;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            pressAnyKeyText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        pressAnyKeyText.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private IEnumerator SelectAndLoadMain()
    {
        isProcessing = true;

        // 깜박임 중단 및 텍스트 보이기
        StopCoroutine(BlinkText());
        if (pressAnyKeyText != null)
            pressAnyKeyText.color = new Color(pressAnyKeyText.color.r, pressAnyKeyText.color.g, pressAnyKeyText.color.b, 1f);

        // 선택 효과음 재생
        if (MenuBGMManager.Instance != null)
        {
            AudioClip clip = MenuBGMManager.Instance.selectClip;
            float volume = MenuBGMManager.Instance.selectVolume;

            if (clip != null)
            {
                MenuBGMManager.Instance.PlaySelectSFX();
                yield return new WaitForSeconds(clip.length);
            }
        }

        // 부드럽게 씬 전환
        SceneManager.LoadScene("Main");
    }
}
