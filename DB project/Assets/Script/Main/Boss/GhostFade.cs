using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFade : MonoBehaviour
{
    public float fadeDuration = 0.5f;       // 잔상(고스트) 사라지는 시간
    private SpriteRenderer sr;              // 스프라이트 렌더러 참조

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); // SpriteRenderer 가져오기
        StartCoroutine(FadeOut());           // 페이드 아웃 코루틴 시작
    }

    System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;                  // 경과 시간 초기화
        Color startColor = sr.color;          // 시작 색상 저장

        while (elapsed < fadeDuration)       // fadeDuration 동안 반복
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeDuration); // 점차 투명하게
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha); // 색상 적용
            elapsed += Time.deltaTime;       // 시간 누적
            yield return null;               // 다음 프레임까지 대기
        }

        Destroy(gameObject);                  // 페이드 완료 후 오브젝트 삭제
    }
}
