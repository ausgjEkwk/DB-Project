using System.Collections;
using UnityEngine;

public class PBossPattern1 : MonoBehaviour
{
    [Header("�̵� ��ǥ")]
    public Vector2[] movePoints = new Vector2[]
    {
        new Vector2(0f, 3f),
        new Vector2(-1f, 4f),
        new Vector2(-1.5f, 2f),
        new Vector2(1f, 4f),
        new Vector2(1.5f, 2f)
    };

    [Header("�ӵ� ����")]
    public float moveSpeed = 3f;

    [Header("�⺻ źȯ ����")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public int bulletsPerCircle = 36;
    public int circleCount = 3;
    public float circleDelay = 0.1f;

    [Header("���� ���� źȯ ����")]
    public GameObject slowBulletPrefab;
    public float slowBulletSpeed = 2f;
    public int slowBulletsPerCircle = 36;
    public int slowCircleCount = 3;
    public float slowCircleDelay = 0.3f;

    private bool isPatternActive = false;
    private Coroutine patternCoroutine;

    void Start()
    {
        // �ڷ�ƾ ���۸� �ϰ� ���� ������ isPatternActive�� ����
        patternCoroutine = StartCoroutine(PatternLoopCoroutine());
    }

    private IEnumerator PatternLoopCoroutine()
    {
        int index = 0;
        while (true)
        {
            if (!isPatternActive)
            {
                yield return null;
                continue;
            }

            Vector2 target = movePoints[index];
            yield return MoveToPosition(target);

            for (int i = 0; i < circleCount; i++)
            {
                ShootCircle();
                yield return new WaitForSeconds(circleDelay);
            }

            for (int i = 0; i < slowCircleCount; i++)
            {
                ShootSlowCircle();
                yield return new WaitForSeconds(slowCircleDelay);
            }

            index = (index + 1) % movePoints.Length;
        }
    }

    private IEnumerator MoveToPosition(Vector2 target)
    {
        while ((Vector2)transform.position != target)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void ShootCircle()
    {
        float angleStep = 360f / bulletsPerCircle;
        for (int i = 0; i < bulletsPerCircle; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            float zRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0f, 0f, zRotation));
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = dir * bulletSpeed;
        }
    }

    private void ShootSlowCircle()
    {
        float angleStep = 360f / slowBulletsPerCircle;
        for (int i = 0; i < slowBulletsPerCircle; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            GameObject bullet = Instantiate(slowBulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = dir * slowBulletSpeed;
        }
    }

    public void SetActivePattern(bool active)
    {
        isPatternActive = active;

        if (!active)
        {
            // ���� ���� �� �ڷ�ƾ ����
            if (patternCoroutine != null)
                StopCoroutine(patternCoroutine);
            patternCoroutine = null;

            // ���� �� 0�� ��ġ�� �ڿ������� �̵�
            StartCoroutine(MoveToPosition(movePoints[0]));
        }
        else
        {
            // ���� ���� �� �ڷ�ƾ ����
            if (patternCoroutine == null)
                patternCoroutine = StartCoroutine(PatternLoopCoroutine());
        }
    }
}
