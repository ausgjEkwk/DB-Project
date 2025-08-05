using System.Collections;
using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    public enum MonsterType { A, B, C }
    public MonsterType type;

    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;

    public Transform player;

    private bool isShooting = false; // 이미 발사 중인지 체크

    void Awake()
    {
        if (player == null)
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
        if (!isShooting)
        {
            isShooting = true;
            StartCoroutine(ShootingRoutine());
        }
    }

    IEnumerator ShootingRoutine()
    {
        while (true) // 무한 반복
        {
            switch (type)
            {
                case MonsterType.A:
                    yield return StartCoroutine(ShootInLowerHalfCircle());
                    break;
                case MonsterType.B:
                    yield return StartCoroutine(ShootVerticalGapToPlayer());
                    break;
                case MonsterType.C:
                    yield return StartCoroutine(ShootTripleVertical());
                    break;
            }
            yield return new WaitForSeconds(1f); // 1초 대기 후 다시 발사
        }
    }

    IEnumerator ShootInLowerHalfCircle()
    {
        int[] bulletCounts = new int[] { 10, 9, 10 };
        float startAngle = 0f;
        float endAngle = -180f;

        for (int line = 0; line < bulletCounts.Length; line++)
        {
            int bulletCount = bulletCounts[line];
            float angleStep = (endAngle - startAngle) / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = startAngle + i * angleStep;
                Vector2 dir = DegreeToVector2(angle).normalized;

                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(dir);
                }

                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.gravityScale = 0f;
                }
            }

            yield return new WaitForSeconds(0.25f); // 줄 사이 n초 대기
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
        Vector2 dir = ((Vector2)player.position - firePos).normalized;

        float gapDistance = 1f;
        float delay = gapDistance / bulletSpeed;

        for (int i = 0; i < 3; i++)
        {
            SpawnBulletFromPosition(firePos, dir);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator ShootTripleVertical()
    {
        int bulletsPerBranch = 5;
        float bulletDelay = 0.1f;

        Vector2[] directions = {
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
                    rb.velocity = directions[branch] * bulletSpeed;
                }
            }
            yield return new WaitForSeconds(bulletDelay);
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
            rb.velocity = dir.normalized * bulletSpeed;
        }
    }

    Vector2 DegreeToVector2(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
}
