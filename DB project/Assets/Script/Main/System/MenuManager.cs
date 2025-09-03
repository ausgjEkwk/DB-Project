using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressAnyKeyText; // "Press Any Key" 텍스트

    [Header("Settings")]
    public float blinkDuration = 1f; // 텍스트 깜박임 속도
    private bool isProcessing = false; // 입력 처리 중인지 여부

    private void Start()
    {
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(true); // 텍스트 활성화
            StartCoroutine(BlinkText()); // 깜박임 코루틴 시작
        }
    }

    private void Update()
    {
        if (isProcessing) return; // 이미 처리 중이면 입력 무시

        // Z 또는 Enter 키 입력 시
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SelectAndLoadMain()); // 씬 전환 코루틴 시작
        }
    }

    // 텍스트 깜박임 코루틴
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

    // 텍스트 페이드 코루틴
    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        if (pressAnyKeyText == null) yield break;

        float time = 0f;
        Color color = pressAnyKeyText.color;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime; // Time.timeScale 영향을 받지 않음
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            pressAnyKeyText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        pressAnyKeyText.color = new Color(color.r, color.g, color.b, endAlpha); // 마지막 알파값 적용
    }

    // 선택 후 씬 전환 코루틴
    private IEnumerator SelectAndLoadMain()
    {
        isProcessing = true; // 입력 중복 방지

        // 깜박임 중단 및 텍스트 표시
        StopCoroutine(BlinkText());
        if (pressAnyKeyText != null)
            pressAnyKeyText.color = new Color(pressAnyKeyText.color.r, pressAnyKeyText.color.g, pressAnyKeyText.color.b, 1f);

        // 선택 효과음 재생 후 대기
        if (MenuBGMManager.Instance != null)
        {
            AudioClip clip = MenuBGMManager.Instance.selectClip;
            float volume = MenuBGMManager.Instance.selectVolume;

            if (clip != null)
            {
                MenuBGMManager.Instance.PlaySelectSFX();
                yield return new WaitForSeconds(clip.length); // 효과음 재생 시간만큼 대기
            }
        }

        // Main 씬으로 전환
        SceneManager.LoadScene("Main");
    }
}
