using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float fadeDuration = 0.5f;       // �ܻ�(��Ʈ) ������� �ð�
    private SpriteRenderer sr;              // ��������Ʈ ������ ����

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); // SpriteRenderer ��������
        StartCoroutine(FadeOut());           // ���̵� �ƿ� �ڷ�ƾ ����
    }

    System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;                  // ��� �ð� �ʱ�ȭ
        Color startColor = sr.color;          // ���� ���� ����

        while (elapsed < fadeDuration)       // fadeDuration ���� �ݺ�
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeDuration); // ���� �����ϰ�
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha); // ���� ����
            elapsed += Time.deltaTime;       // �ð� ����
            yield return null;               // ���� �����ӱ��� ���
        }

        Destroy(gameObject);                  // ���̵� �Ϸ� �� ������Ʈ ����
    }
}
