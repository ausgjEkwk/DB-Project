using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpecial : MonoBehaviour
{
    [Header("Settings")]
    public GameObject swordPrefab;       // 특수패턴에서 발사할 검 프리팹
    public float moveSpeed = 2f;         // 보스 이동 속도 (시간정지 상태와 무관)
    public float stepDistance = 0.5f;    // 한 번에 이동할 거리
    public float leftX = -2.5f;          // 특수패턴 좌측 끝 X좌표
    public float rightX = 2.5f;          // 특수패턴 우측 끝 X좌표

    private bool isRunning = false;      // 특수패턴 실행 중 여부
    private bool isFinished = false;     // 특수패턴 완료 여부

    public bool IsRunning => isRunning;  // 외부에서 실행 중인지 확인용
    public bool IsFinished => isFinished; // 외부에서 완료 여부 확인용

    private bool isPaused = false;       // 탄환 이동 일시정지 상태
    private List<GameObject> activeSwords = new List<GameObject>(); // 현재 생성된 검 리스트

    public bool DisableOtherPatterns => isRunning; // 패턴 실행 중 다른 패턴 비활성화 여부

    // 특수패턴 시작 시도 (콜백으로 완료 알림)
    public void TryStartSpecial(Action onComplete)
    {
        if (isRunning || isFinished) return; // 이미 실행 중이거나 완료된 경우 무시

        ClearExistingSwords(); // 기존 검 삭제
        StopAllCoroutines(); // 모든 코루틴 중단
        StartCoroutine(StartSpecialRoutine(onComplete)); // 특수패턴 시작
    }

    // 기존 생성된 검 삭제
    private void ClearExistingSwords()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(); // 모든 오브젝트 탐색
        foreach (var obj in allObjects)
        {
            if (obj.name == "Bossbullet" || obj.name.StartsWith("Bossbullet(Clone)"))
                Destroy(obj); // 기존 검 삭제
        }
        activeSwords.Clear(); // 리스트 초기화
    }

    // 특수패턴 코루틴
    private IEnumerator StartSpecialRoutine(Action onComplete)
    {
        isRunning = true; // 실행 중 상태

        // 좌측 끝으로 이동
        yield return MoveToPosition(new Vector3(leftX, transform.position.y, transform.position.z));

        Vector3 currentPos = transform.position;

        while (currentPos.x < rightX) // 우측 끝까지 이동
        {
            while (IsPausedOrStopped()) // ESC 일시정지 대기
                yield return null;

            Vector3 nextPos = currentPos + new Vector3(stepDistance, 0f, 0f); // 다음 위치 계산
            if (nextPos.x > rightX) nextPos.x = rightX; // 우측 제한

            yield return MoveToPosition(nextPos); // 이동
            currentPos = nextPos;

            while (IsPausedOrStopped()) // 일시정지 확인
                yield return null;

            if (TimeStop.Instance != null)
                TimeStop.Instance.StartTimeStop(); // 시간정지 시작

            FireBulletsInLowerHalfCircle(); // 하단 반원 발사

            while (IsPausedOrStopped()) // 일시정지 확인
                yield return null;

            // 다음 이동
            nextPos = currentPos + new Vector3(stepDistance, 0f, 0f);
            if (nextPos.x > rightX) nextPos.x = rightX;

            yield return MoveToPosition(nextPos); // 이동
            currentPos = nextPos;

            if (TimeStop.Instance != null)
                TimeStop.Instance.EndTimeStop(); // 시간정지 종료
        }

        // 가운데로 이동
        while (IsPausedOrStopped())
            yield return null;

        yield return MoveToPosition(new Vector3(0f, transform.position.y, transform.position.z));

        isRunning = false; // 실행 종료
        isFinished = true; // 완료 상태
        onComplete?.Invoke(); // 완료 콜백 호출
    }

    // ESC 일시정지 상태 체크
    private bool IsPausedOrStopped()
    {
        return GamePauseUIManager.Instance != null && GamePauseUIManager.Instance.IsShown;
    }

    // 지정 위치로 이동 (일시정지 적용)
    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f) // 목표 위치 도달 전까지
        {
            while (IsPausedOrStopped())
                yield return null; // 일시정지 대기

            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime); // 이동
            yield return null;
        }
        transform.position = target; // 정확히 위치 맞춤
    }

    // 하단 반원으로 검 발사
    private void FireBulletsInLowerHalfCircle()
    {
        if (IsPausedOrStopped()) return;

        int bulletCount = 10; // 발사할 총알 수
        float startAngle = 0f; // 시작 각도
        float endAngle = -180f; // 끝 각도
        float angleStep = (endAngle - startAngle) / (bulletCount - 1); // 각도 간격

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 dir = DegreeToVector2(angle).normalized; // 방향 벡터 계산

            GameObject bullet = Instantiate(swordPrefab, transform.position, Quaternion.identity); // 검 생성
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = true; // 물리 영향 없음
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;
            }

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f; // 회전 계산
            bullet.transform.rotation = Quaternion.Euler(0, 0, rotZ); // 회전 적용

            activeSwords.Add(bullet); // 리스트에 추가

            StartCoroutine(MoveSwordBullet(bullet.transform, dir, 5f)); // 이동 코루틴 실행
        }
    }

    // 개별 검 이동 코루틴
    private IEnumerator MoveSwordBullet(Transform bulletTransform, Vector2 direction, float speed)
    {
        float lifeTime = 3f; // 이동 시간
        float timer = 0f;

        while (timer < lifeTime && bulletTransform != null)
        {
            while (IsPausedOrStopped())
                yield return null; // 일시정지 대기

            if (!isPaused) // 이동 가능 상태면
            {
                bulletTransform.Translate(direction * speed * Time.deltaTime, Space.World); // 이동
                timer += Time.deltaTime;
            }
            yield return null;
        }

        if (bulletTransform != null)
            Destroy(bulletTransform.gameObject); // 생명 종료 후 삭제
    }

    // 각도를 단위 벡터로 변환
    private Vector2 DegreeToVector2(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    // 검 이동 일시정지
    public void PauseBullets()
    {
        isPaused = true;
    }

    // 검 이동 재개
    public void ResumeBullets()
    {
        isPaused = false;
    }
}
