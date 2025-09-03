using System;
using System.Collections;
using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    public enum MonsterType { A, B, C }        // 몬스터 타입(A,B,C)
    public MonsterType type;

    public GameObject bulletPrefab;             // 발사할 총알 프리팹
    public float bulletSpeed = 5f;             // 총알 속도

    public Transform player;                    // 플레이어 위치 참조

    private bool isShooting = false;           // 이미 발사 중인지 체크

    void Awake()
    {
        if (player == null)                     // player 미지정 시 자동 할당
        {
            GameObject pObj = GameObject.FindWithTag("Player");
            if (pObj != null)
                player = pObj.transform;
            else
                Debug.LogWarning("Player object not found. Please check Player tag!");
        }
    }

    public void StartShooting()
    {
        if (!isShooting)                       // 중복 실행 방지
        {
            isShooting = true;
            StartCoroutine(ShootingRoutine()); // 발사 루틴 시작
        }
    }

    IEnumerator ShootingRoutine()
    {
        while (true)                            // 무한 루프
        {
            switch (type)
            {
                case MonsterType.A:
                    yield return StartCoroutine(ShootInLowerHalfCircle()); // A 타입 패턴
                    break;
                case MonsterType.B:
                    yield return StartCoroutine(ShootVerticalGapToPlayer()); // B 타입 패턴
                    break;
                case MonsterType.C:
                    yield return StartCoroutine(ShootTripleVertical()); // C 타입 패턴
                    break;
            }
            yield return new WaitForSeconds(1f); // 다음 발사까지 1초 대기
        }
    }

    IEnumerator ShootInLowerHalfCircle()
    {
        int[] bulletCounts = new int[] { 10, 9, 10 }; // 각 줄별 총알 수
        float startAngle = 0f;                       // 시작 각도
        float endAngle = -180f;                      // 끝 각도 (아래 방향)

        for (int line = 0; line < bulletCounts.Length; line++)
        {
            int bulletCount = bulletCounts[line];
            float angleStep = (endAngle - startAngle) / (bulletCount - 1); // 각도 간격

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = startAngle + i * angleStep;
                Vector2 dir = DegreeToVector2(angle).normalized;          // 각도 -> 방향 벡터

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(dir);                        // 총알 방향 설정
                }

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;               // Rigidbody 설정
                    rb.gravityScale = 0f;
                }
            }

            yield return new WaitForSeconds(0.25f); // 줄 사이 대기
        }
    }

    IEnumerator ShootVerticalGapToPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player Transform not found!");
            yield break;
        }

        Vector2 firePos = transform.position;
        Vector2 dir = ((Vector2)player.position - firePos).normalized; // 플레이어 방향

        float gapDistance = 1f;                     // 총알 간 거리
        float delay = gapDistance / bulletSpeed;    // 발사 간격 계산

        for (int i = 0; i < 3; i++)
        {
            SpawnBulletFromPosition(firePos, dir); // 총알 생성
            yield return new WaitForSeconds(delay); // 발사 간격
        }
    }

    IEnumerator ShootTripleVertical()
    {
        int bulletsPerBranch = 5;                   // 3갈래 당 총알 수
        float bulletDelay = 0.1f;                   // 총알 간 간격

        Vector2[] directions = {                     // 3갈래 방향
            new Vector2(-1f, -1f).normalized,
            Vector2.down,
            new Vector2(1f, -1f).normalized
        };

        for (int i = 0; i < bulletsPerBranch; i++)
        {
            for (int branch = 0; branch < directions.Length; branch++)
            {
                Vector2 spawnPos = transform.position;

                GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.gravityScale = 0f;
                    rb.velocity = directions[branch] * bulletSpeed; // 총알 속도 적용
                }
            }
            yield return new WaitForSeconds(bulletDelay);        // 발사 간 간격
        }
    }

    void SpawnBulletFromPosition(Vector2 position, Vector2 dir)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.velocity = dir.normalized * bulletSpeed; // 방향 적용
        }
    }

    Vector2 DegreeToVector2(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)); // 각도 -> 벡터
    }
}
