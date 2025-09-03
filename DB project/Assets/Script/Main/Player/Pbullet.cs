using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pbullet : MonoBehaviour
{
    private Rigidbody2D rb;          // 총알 Rigidbody2D 참조
    private Vector2 savedVelocity;   // 시간정지 전 속도 저장

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 가져오기
        savedVelocity = rb.velocity;       // 초기 속도 저장
    }

    void Update()
    {
        // 시간정지 상태일 때
        if (TimeStop.Instance != null && TimeStop.Instance.IsTimeStopped)
        {
            if (rb.velocity != Vector2.zero) // 아직 이동 중이면
            {
                savedVelocity = rb.velocity; // 현재 속도 저장
                rb.velocity = Vector2.zero;  // 속도 0으로 정지
                rb.isKinematic = true;       // 물리 영향 제거
            }
        }
        else // 시간정지 해제 시
        {
            if (rb.velocity == Vector2.zero) // 이전에 정지된 상태면
            {
                rb.isKinematic = false;      // 물리 영향 다시 적용
                rb.velocity = savedVelocity; // 저장된 속도로 이동 재개
            }
        }
    }
}
