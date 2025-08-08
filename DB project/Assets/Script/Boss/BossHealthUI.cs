using UnityEngine;

public class BossHealthUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private float fullWidth;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform 컴포넌트를 찾을 수 없습니다!");
        }
        else
        {
            fullWidth = rectTransform.sizeDelta.x;
        }
    }

    public void SetHealthPercent(float percent)
    {
        if (rectTransform == null) return;

        percent = Mathf.Clamp01(percent);
        Vector2 size = rectTransform.sizeDelta;
        size.x = fullWidth * percent;
        rectTransform.sizeDelta = size;
    }
}
