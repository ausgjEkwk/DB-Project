// BossPattern1.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern1 : MonoBehaviour
{
    public GameObject bossBulletPrefab;
    public float bulletDistance = 1.5f;

    public float rotateSpeed = 360f;
    public float totalRotations = 3f;
    public float fireRate = 0.05f;

    public float launchInterval = 0.05f;
    public float bulletSpeed = 7f;

    private List<GameObject> spawnedBullets = new List<GameObject>();

    private BossSpecial bossSpecial;

    void Start()
    {
        bossSpecial = GetComponent<BossSpecial>();
        if (bossSpecial == null)
        {
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
        }
    }

    public IEnumerator StartPattern()
    {
        if (bossSpecial != null && bossSpecial.IsRunning)
        {
            yield break;
        }

        yield return StartCoroutine(FirePattern());
    }

    IEnumerator FirePattern()
    {
        float elapsed = 0f;
        float angle = 0f;
        float fireCooldown = 0f;

        float duration = 360f * totalRotations / rotateSpeed;

        List<Vector2> bulletPositions = new List<Vector2>();
        List<Vector2> bulletDirections = new List<Vector2>();

        while (elapsed < duration)
        {
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
                for (int i = 0; i < 4; i++)
                {
                    float currentAngle = angle + i * 90f;
                    Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad)).normalized;

                    Vector2 spawnPos = (Vector2)transform.position;

                    GameObject bullet = Instantiate(bossBulletPrefab, spawnPos, Quaternion.identity);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                    if (rb != null)
                        rb.velocity = Vector2.zero;

                    bullet.transform.up = dir;

                    spawnedBullets.Add(bullet);

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

        yield return new WaitForSeconds(1f);

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
