// BossSpecial.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpecial : MonoBehaviour
{
    [Header("Settings")]
    public GameObject swordPrefab;       // Ư�����Ͽ��� �߻��� �� ������
    public float moveSpeed = 2f;         // ���� �̵� �ӵ� (�ð����� ���¿� ������ �̵� �ӵ�)
    public float stepDistance = 0.5f;    // �̵� �� �ѹ��� �����̴� �Ÿ�
    public float leftX = -2.5f;          // Ư������ ���� �� X��ǥ
    public float rightX = 2.5f;          // Ư������ ���� �� X��ǥ

    private bool isRunning = false;      // Ư������ ���������� ����
    private bool isFinished = false;     // Ư������ �Ϸ� ����

    public bool IsRunning => isRunning;
    public bool IsFinished => isFinished;

    private bool isPaused = false;       // źȯ �̵� �Ͻ����� ���� (�ð�������)
    private List<GameObject> activeSwords = new List<GameObject>();

    public bool DisableOtherPatterns => isRunning;

    // Ư������ ���� �õ� (�ݹ����� �Ϸ� �˸�)
    public void TryStartSpecial(Action onComplete)
    {
        if (isRunning || isFinished) return;

        ClearExistingSwords();
        StopAllCoroutines();
        StartCoroutine(StartSpecialRoutine(onComplete));
    }

    private void ClearExistingSwords()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name == "Bossbullet" || obj.name.StartsWith("Bossbullet(Clone)"))
                Destroy(obj);
        }
        activeSwords.Clear();
    }

    private IEnumerator StartSpecialRoutine(Action onComplete)
    {
        isRunning = true;

        yield return MoveToPositionUnscaled(new Vector3(leftX, transform.position.y, transform.position.z));

        Vector3 currentPos = transform.position;

        while (currentPos.x < rightX)
        {
            Vector3 nextPos = currentPos + new Vector3(stepDistance, 0f, 0f);
            if (nextPos.x > rightX) nextPos.x = rightX;
            yield return MoveToPositionUnscaled(nextPos);
            currentPos = nextPos;

            if (TimeStop.Instance != null)
                TimeStop.Instance.StartTimeStop();

            FireBulletsInLowerHalfCircle();

            nextPos = currentPos + new Vector3(stepDistance, 0f, 0f);
            if (nextPos.x > rightX) nextPos.x = rightX;
            yield return MoveToPositionUnscaled(nextPos);
            currentPos = nextPos;

            if (TimeStop.Instance != null)
                TimeStop.Instance.EndTimeStop();
        }

        yield return MoveToPositionUnscaled(new Vector3(0f, transform.position.y, transform.position.z));

        isRunning = false;
        isFinished = true;

        onComplete?.Invoke();
    }

    private IEnumerator MoveToPositionUnscaled(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
        transform.position = target;
    }

    private void FireBulletsInLowerHalfCircle()
    {
        int bulletCount = 10;
        float startAngle = 0f;
        float endAngle = -180f;
        float angleStep = (endAngle - startAngle) / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 dir = DegreeToVector2(angle).normalized;

            GameObject bullet = Instantiate(swordPrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;
            }

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            bullet.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            activeSwords.Add(bullet);

            StartCoroutine(MoveSwordBullet(bullet.transform, dir, 5f));
        }
    }

    private IEnumerator MoveSwordBullet(Transform bulletTransform, Vector2 direction, float speed)
    {
        float lifeTime = 3f;
        float timer = 0f;

        while (timer < lifeTime && bulletTransform != null)
        {
            if (!isPaused)
            {
                bulletTransform.Translate(direction * speed * Time.deltaTime, Space.World);
                timer += Time.deltaTime;
            }
            yield return null;
        }

        if (bulletTransform != null)
            Destroy(bulletTransform.gameObject);
    }

    private Vector2 DegreeToVector2(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public void PauseBullets()
    {
        isPaused = true;
    }

    public void ResumeBullets()
    {
        isPaused = false;
    }
}
