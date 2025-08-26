using UnityEngine;

public class BossHealthUI : MonoBehaviour
{
    private RectTransform rectTransform; // 체력바 RectTransform 참조
    private float fullWidth;             // 체력바 최대 너비

    void Awake()
    {
        // RectTransform 컴포넌트 가져오기
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform 컴포넌트를 찾을 수 없습니다!");
        }
        else
        {
            fullWidth = rectTransform.sizeDelta.x; // 초기 전체 너비 저장
        }
    }

    // 체력 비율(percent) 업데이트
    public void SetHealthPercent(float percent)
    {
        if (rectTransform == null) return;

        // 0~1 범위로 제한
        percent = Mathf.Clamp01(percent);

        // RectTransform 가로 길이(percent 비율)로 조정
        Vector2 size = rectTransform.sizeDelta;
        size.x = fullWidth * percent;
        rectTransform.sizeDelta = size;
    }
}
