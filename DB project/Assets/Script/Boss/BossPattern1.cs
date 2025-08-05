using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern1 : MonoBehaviour
{
    public GameObject bossBulletPrefab;   // 발사할 프리팹 (칼)
    public float bulletDistance = 1.5f;   // 보스로부터의 거리

    public float rotateSpeed = 360f;      // 초당 회전각 (도)
    public float totalRotations = 3f;     // 총 회전 바퀴 수 (3바퀴)
    public float fireRate = 0.05f;        // 생성 간격 (초)

    public float launchInterval = 0.05f;  // 발사 간격 (초)
    public float bulletSpeed = 7f;        // 칼 이동 속도 및 발사 속도

    private List<GameObject> spawnedBullets = new List<GameObject>();

    public IEnumerator StartPattern()
    {
        yield return StartCoroutine(FirePattern());
    }

    IEnumerator FirePattern()
    {
        float elapsed = 0f;
        float angle = 0f;
        float fireCooldown = 0f;

        float duration = 360f * totalRotations / rotateSpeed;

        // 칼 위치 및 방향 저장 (발사용)
        List<Vector2> bulletPositions = new List<Vector2>();
        List<Vector2> bulletDirections = new List<Vector2>();

        while (elapsed < duration)
        {
            // 생성된 모든 칼을 목표 위치로 부드럽게 이동
            foreach (GameObject bullet in spawnedBullets)
            {
                if (bullet != null)
                {
                    Vector2 targetPos = (Vector2)transform.position + (Vector2)bullet.transform.up * bulletDistance;
                    bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, targetPos, bulletSpeed * Time.deltaTime);
                }
            }

            if (fireCooldown <= 0f)
            {
                // 4개의 칼을 90도 간격으로 생성 (현재 angle 기준 회전)
                for (int i = 0; i < 4; i++)
                {
                    float currentAngle = angle + i * 90f;
                    Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;

                    Vector2 spawnPos = (Vector2)transform.position;  // 보스 중심에서 생성

                    GameObject bullet = Instantiate(bossBulletPrefab, spawnPos, Quaternion.identity);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                        rb.velocity = Vector2.zero;

                    bullet.transform.up = dir;

                    spawnedBullets.Add(bullet);

                    // 발사 위치와 방향 저장
                    bulletPositions.Add((Vector2)transform.position + dir * bulletDistance);
                    bulletDirections.Add(dir);
                }
                fireCooldown = fireRate;
            }

            yield return null;
            elapsed += Time.deltaTime;
            fireCooldown -= Time.deltaTime;
            angle += rotateSpeed * Time.deltaTime;
        }

        // while 루프 종료 후, 모든 칼이 목표 위치에 도달할 때까지 부드럽게 이동
        bool allReached = false;
        while (!allReached)
        {
            allReached = true;
            foreach (GameObject bullet in spawnedBullets)
            {
                if (bullet != null)
                {
                    Vector2 targetPos = (Vector2)transform.position + (Vector2)bullet.transform.up * bulletDistance;
                    bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, targetPos, bulletSpeed * Time.deltaTime);

                    if (Vector2.Distance(bullet.transform.position, targetPos) > 0.01f)
                        allReached = false;
                }
            }
            yield return null;
        }

        // 보스가 발사 끝내고 1초 대기
        yield return new WaitForSeconds(1f);

        // 칼들을 저장된 위치로 이동시키고 순차 발사
        for (int i = 0; i < spawnedBullets.Count; i++)
        {
            GameObject bullet = spawnedBullets[i];
            if (bullet != null)
            {
                bullet.transform.position = bulletPositions[i];
                bullet.transform.up = bulletDirections[i];

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = bulletDirections[i] * bulletSpeed;
                }
            }
            yield return new WaitForSeconds(launchInterval);
        }

        spawnedBullets.Clear();
    }
}
