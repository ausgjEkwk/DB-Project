using System.Collections;
using TMPro;
using UnityEngine;

public class PressAnyKeyBlink : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText; // "Press Any Key" 텍스트
    public float blinkSpeed = 1f;         // 깜박임 속도

    private void Start()
    {
        if (pressKeyText != null)
            StartCoroutine(BlinkText()); // 깜박임 코루틴 시작
    }

    // 텍스트 알파값을 PingPong으로 반복해서 깜박이게 함
    private IEnumerator BlinkText()
    {
        Color originalColor = pressKeyText.color; // 원래 색상 저장

        while (true) // 무한 반복
        {
            // 0 ~ 1 알파값 반복
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            pressKeyText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null; // 다음 프레임까지 대기
        }
    }
}
