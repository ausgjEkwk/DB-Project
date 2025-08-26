using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI pressAnyKeyText; // "Press Any Key" �ؽ�Ʈ

    [Header("Settings")]
    public float blinkDuration = 1f; // �ؽ�Ʈ ������ �ӵ�
    private bool isProcessing = false; // �Է� ó�� ������ ����

    private void Start()
    {
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.gameObject.SetActive(true); // �ؽ�Ʈ Ȱ��ȭ
            StartCoroutine(BlinkText()); // ������ �ڷ�ƾ ����
        }
    }

    private void Update()
    {
        if (isProcessing) return; // �̹� ó�� ���̸� �Է� ����

        // Z �Ǵ� Enter Ű �Է� ��
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(SelectAndLoadMain()); // �� ��ȯ �ڷ�ƾ ����
        }
    }

    // �ؽ�Ʈ ������ �ڷ�ƾ
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

    // �ؽ�Ʈ ���̵� �ڷ�ƾ
    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        if (pressAnyKeyText == null) yield break;

        float time = 0f;
        Color color = pressAnyKeyText.color;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime; // Time.timeScale ������ ���� ����
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            pressAnyKeyText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        pressAnyKeyText.color = new Color(color.r, color.g, color.b, endAlpha); // ������ ���İ� ����
    }

    // ���� �� �� ��ȯ �ڷ�ƾ
    private IEnumerator SelectAndLoadMain()
    {
        isProcessing = true; // �Է� �ߺ� ����

        // ������ �ߴ� �� �ؽ�Ʈ ǥ��
        StopCoroutine(BlinkText());
        if (pressAnyKeyText != null)
            pressAnyKeyText.color = new Color(pressAnyKeyText.color.r, pressAnyKeyText.color.g, pressAnyKeyText.color.b, 1f);

        // ���� ȿ���� ��� �� ���
        if (MenuBGMManager.Instance != null)
        {
            AudioClip clip = MenuBGMManager.Instance.selectClip;
            float volume = MenuBGMManager.Instance.selectVolume;

            if (clip != null)
            {
                MenuBGMManager.Instance.PlaySelectSFX();
                yield return new WaitForSeconds(clip.length); // ȿ���� ��� �ð���ŭ ���
            }
        }

        // Main ������ ��ȯ
        SceneManager.LoadScene("Main");
    }
}
