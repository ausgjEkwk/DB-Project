using System.Collections;
using TMPro;
using UnityEngine;

public class PressAnyKeyBlink : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText;
    public float blinkSpeed = 1f; // ������ �ӵ�

    private void Start()
    {
        if (pressKeyText != null)
            StartCoroutine(BlinkText());
    }

    private IEnumerator BlinkText()
    {
        Color originalColor = pressKeyText.color;

        while (true)
        {
            // PingPong�� �̿��� 0~1 ���� �� �ݺ�
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            pressKeyText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }
    }
}
