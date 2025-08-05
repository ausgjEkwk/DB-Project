using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern1 : MonoBehaviour
{
    public GameObject bossBulletPrefab;   // 발사할 프리팹 (칼)
    public float bulletDistance = 1.5f;     // 보스로부터의 거리

    public float rotateSpeed = 360f;      // 초당 회전각 (도)
    public float totalRotations = 2f;     // 총 회전 바퀴 수 (2바퀴)
    public float fireRate = 0.05f;        // 생성 간격 (초)

    public float launchInterval = 0.05f;  // 발사 간격 (초)
    public float bulletSpeed = 7f;        // 발사 속도

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

        // 칼 생성 위치 및 방향 저장
        List<Vector2> bulletPositions = new List<Vector2>();
        List<Vector2> bulletDirections = new List<Vector2>();

        // 칼들을 빠르게 회전하면서 차례대로 생성
        while (elapsed < duration)
        {
            if (fireCooldown <= 0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    float currentAngle = angle + i * 90f;
                    Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;
                    Vector2 spawnPos = (Vector2)transform.position + dir * bulletDistance;

                    GameObject bullet = Instantiate(bossBulletPrefab, spawnPos, Quaternion.identity);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                        rb.velocity = Vector2.zero; // 정지 상태

                    bullet.transform.up = dir; // 방향 설정

                    spawnedBullets.Add(bullet);
                    bulletPositions.Add(spawnPos);
                    bulletDirections.Add(dir);
                }
                fireCooldown = fireRate;
            }

            yield return null;
            elapsed += Time.deltaTime;
            fireCooldown -= Time.deltaTime;
            angle += rotateSpeed * Time.deltaTime;
        }

        // 보스가 발사 끝내고 1초 대기
        yield return new WaitForSeconds(1f);

        // 칼들을 고정 위치로 재배치하고 순차 발사
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
