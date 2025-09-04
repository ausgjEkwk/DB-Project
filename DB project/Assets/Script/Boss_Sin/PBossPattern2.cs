using System.Collections;
using UnityEngine;

public class PBossPattern2 : MonoBehaviour
{
    [Header("마법진 프리팹")]
    public GameObject magicCirclePrefab;

    [Header("탄환 프리팹")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;

    [Header("발사 간격 (이동 거리 기준)")]
    public float fireDistanceInterval = 0.1f; // Inspector에서 조절 가능

    [Header("마법진 시작 위치")]
    public Vector2[] loopPositions = new Vector2[4]
    {
        new Vector2(-3.3f, 6f),   // 0
        new Vector2(-3.3f, -6f),  // 1
        new Vector2(3.3f, -6f),   // 2
        new Vector2(3.3f, 6f)     // 3
    };

    [Header("이동 속도 기준 (1초당 이동)")]
    public float moveDuration = 1.5f;

    private Transform[] magicCircles = new Transform[4];
    private int[] indices = new int[4]; // 각 마법진이 참조하는 loopPositions 인덱스

    void Start()
    {
        // 마법진 생성 & 초기 인덱스 세팅
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = Instantiate(magicCirclePrefab, loopPositions[i], Quaternion.identity, this.transform);
            magicCircles[i] = obj.transform;
            indices[i] = i;
        }

        StartCoroutine(MoveLoop());
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            // 목표 위치 계산
            Vector2[] targetPositions = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                // 1,2번 → 시계방향
                if (i == 0 || i == 1)
                    targetPositions[i] = loopPositions[(indices[i] + 1) % loopPositions.Length];
                // 3,4번 → 반시계방향
                else
                    targetPositions[i] = loopPositions[(indices[i] - 1 + loopPositions.Length) % loopPositions.Length];
            }

            // 이동 시작
            float t = 0f;
            Vector3[] startPos = new Vector3[4];
            Vector3[] prevPos = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                startPos[i] = magicCircles[i].position;
                prevPos[i] = startPos[i];
            }

            while (t < 1f)
            {
                t += Time.deltaTime / moveDuration;

                for (int i = 0; i < 4; i++)
                {
                    magicCircles[i].position = Vector3.Lerp(startPos[i], targetPositions[i], t);

                    float moved = Vector3.Distance(prevPos[i], magicCircles[i].position);
                    if (moved >= fireDistanceInterval)
                    {
                        FireBullet(magicCircles[i].position);
                        prevPos[i] = magicCircles[i].position;
                    }
                }
                yield return null;
            }

            // 도착 보정
            for (int i = 0; i < 4; i++)
                magicCircles[i].position = targetPositions[i];

            // 인덱스 갱신
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 1)
                    indices[i] = (indices[i] + 1) % loopPositions.Length; // 시계
                else
                    indices[i] = (indices[i] - 1 + loopPositions.Length) % loopPositions.Length; // 반시계
            }
        }
    }

    private void FireBullet(Vector3 spawnPos)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (Vector2.zero - (Vector2)spawnPos).normalized;
            rb.velocity = dir * bulletSpeed;
        }
    }
}
