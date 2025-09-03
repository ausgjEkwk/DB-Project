using System.Collections; // 코루틴 사용
using System.Collections.Generic; // 리스트 사용
using UnityEngine;

public class BossPattern1 : MonoBehaviour
{
    public GameObject bossBulletPrefab; // 발사할 보스 총알 프리팹
    public float bulletDistance = 1.5f; // 총알이 이동할 목표 거리
    public float rotateSpeed = 360f; // 보스 회전 속도
    public float totalRotations = 3f; // 총 회전 횟수
    public float fireRate = 0.05f; // 총알 발사 간격
    public float launchInterval = 0.05f; // 최종 발사 간격
    public float bulletSpeed = 7f; // 총알 이동 속도

    private List<GameObject> spawnedBullets = new List<GameObject>(); // 생성된 총알 리스트
    private BossSpecial bossSpecial; // 특수패턴 실행 여부 확인
    private BossMovement bossMovement; // 보스 이동/패턴 관리 참조

    void Start()
    {
        bossSpecial = GetComponent<BossSpecial>(); // BossSpecial 참조
        bossMovement = GetComponent<BossMovement>(); // BossMovement 참조

        if (bossSpecial == null) // 없으면 경고
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
        if (bossMovement == null) // 없으면 경고
            Debug.LogWarning("BossMovement 컴포넌트가 없습니다. 칼 등록 불가.");
    }

    public IEnumerator StartPattern() // 패턴 시작 코루틴
    {
        yield return StartCoroutine(FirePattern()); // 총알 발사 패턴 실행
    }

    IEnumerator FirePattern() // 총알 회전 발사 패턴
    {
        float elapsed = 0f; // 경과 시간
        float angle = 0f; // 현재 회전 각도
        float fireCooldown = 0f; // 발사 쿨다운
        float duration = 360f * totalRotations / rotateSpeed; // 패턴 지속 시간

        List<Vector2> bulletPositions = new List<Vector2>(); // 최종 위치 저장
        List<Vector2> bulletDirections = new List<Vector2>(); // 방향 저장

        while (elapsed < duration) // 패턴 진행 루프
        {
            if (bossSpecial != null && bossSpecial.IsRunning) // 특수패턴 중이면 대기
            {
                yield return null;
                elapsed += Time.deltaTime;
                continue;
            }

            foreach (GameObject bullet in spawnedBullets) // 기존 총알 이동
            {
                if (bullet != null)
                {
                    Vector2 targetPos = (Vector2)transform.position + (Vector2)bullet.transform.up * bulletDistance; // 목표 위치 계산
                    bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, targetPos, bulletSpeed * Time.deltaTime); // 이동
                }
            }

            if (fireCooldown <= 0f) // 쿨다운 끝나면 새 총알 생성
            {
                for (int i = 0; i < 4; i++) // 4방향 총알 생성
                {
                    float currentAngle = angle + i * 90f; // 각도 계산
                    Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized; // 방향 벡터
                    Vector2 spawnPos = (Vector2)transform.position; // 생성 위치

                    GameObject bullet = Instantiate(bossBulletPrefab, spawnPos, Quaternion.identity); // 총알 생성

                    if (bossMovement != null) // 보스 패턴에 등록
                        bossMovement.RegisterPatternSword(bullet);

                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); // Rigidbody 참조
                    if (rb != null)
                        rb.velocity = Vector2.zero; // 초기 속도 0

                    bullet.transform.up = dir; // 총알 방향 설정

                    spawnedBullets.Add(bullet); // 리스트에 저장
                    bulletPositions.Add((Vector2)transform.position + dir * bulletDistance); // 목표 위치 저장
                    bulletDirections.Add(dir); // 방향 저장
                }
                fireCooldown = fireRate; // 쿨다운 초기화
            }

            yield return null; // 한 프레임 대기
            elapsed += Time.deltaTime; // 경과 시간 갱신
            fireCooldown -= Time.deltaTime; // 쿨다운 감소
            angle += rotateSpeed * Time.deltaTime; // 회전 각도 증가
        }

        bool allReached = false; // 최종 이동 확인
        while (!allReached) // 총알 목표 위치 도달할 때까지 반복
        {
            allReached = true;
            foreach (GameObject bullet in spawnedBullets)
            {
                if (bullet != null)
                {
                    Vector2 targetPos = (Vector2)transform.position + (Vector2)bullet.transform.up * bulletDistance; // 목표 위치 계산
                    bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, targetPos, bulletSpeed * Time.deltaTime); // 이동

                    if (Vector2.Distance(bullet.transform.position, targetPos) > 0.01f) // 도달 여부 확인
                        allReached = false;
                }
            }
            yield return null; // 한 프레임 대기
        }

        yield return new WaitForSeconds(1f); // 잠시 대기

        for (int i = 0; i < spawnedBullets.Count; i++) // 최종 발사
        {
            GameObject bullet = spawnedBullets[i];
            if (bullet != null)
            {
                bullet.transform.position = bulletPositions[i]; // 위치 보정
                bullet.transform.up = bulletDirections[i]; // 방향 보정

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.velocity = bulletDirections[i] * bulletSpeed; // 발사
            }
            yield return new WaitForSeconds(launchInterval); // 간격
        }

        spawnedBullets.Clear(); // 리스트 초기화
    }
}
