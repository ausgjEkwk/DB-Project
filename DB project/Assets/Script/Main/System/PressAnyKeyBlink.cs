using System.Collections;
using TMPro;
using UnityEngine;

public class PressAnyKeyBlink : MonoBehaviour
{
    public TextMeshProUGUI pressKeyText; // "Press Any Key" �ؽ�Ʈ
    public float blinkSpeed = 1f;         // ������ �ӵ�

    private void Start()
    {
        if (pressKeyText != null)
            StartCoroutine(BlinkText()); // ������ �ڷ�ƾ ����
    }

    // �ؽ�Ʈ ���İ��� PingPong���� �ݺ��ؼ� �����̰� ��
    private IEnumerator BlinkText()
    {
        Color originalColor = pressKeyText.color; // ���� ���� ����

        while (true) // ���� �ݺ�
        {
            // 0 ~ 1 ���İ� �ݺ�
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            pressKeyText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null; // ���� �����ӱ��� ���
        }
    }
}
