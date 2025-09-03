using UnityEngine;

public class BossHealthUI : MonoBehaviour
{
    private RectTransform rectTransform; // ü�¹� RectTransform ����
    private float fullWidth;             // ü�¹� �ִ� �ʺ�

    void Awake()
    {
        // RectTransform ������Ʈ ��������
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform ������Ʈ�� ã�� �� �����ϴ�!");
        }
        else
        {
            fullWidth = rectTransform.sizeDelta.x; // �ʱ� ��ü �ʺ� ����
        }
    }

    // ü�� ����(percent) ������Ʈ
    public void SetHealthPercent(float percent)
    {
        if (rectTransform == null) return;

        // 0~1 ������ ����
        percent = Mathf.Clamp01(percent);

        // RectTransform ���� ����(percent ����)�� ����
        Vector2 size = rectTransform.sizeDelta;
        size.x = fullWidth * percent;
        rectTransform.sizeDelta = size;
    }
}
