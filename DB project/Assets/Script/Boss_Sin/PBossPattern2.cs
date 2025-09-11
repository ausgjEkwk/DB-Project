using System.Collections;
using UnityEngine;

public class PBossPattern2 : MonoBehaviour
{
    [Header("������ ������")]
    public GameObject magicCirclePrefab;

    [Header("źȯ ������")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;

    [Header("�߻� ���� (�̵� �Ÿ� ����)")]
    public float fireDistanceInterval = 0.1f;

    [Header("������ ���� ��ġ")]
    public Vector2[] startPositions = new Vector2[4]
    {
        new Vector2(-3.3f, 6f),
        new Vector2(-3.3f, -6f),
        new Vector2(3.3f, -6f),
        new Vector2(3.3f, 6f)
    };

    [Header("�̵� �ӵ� ���� (1�ʴ� �̵�)")]
    public float moveDuration = 1.5f;

    private Transform[] magicCircles = new Transform[4];
    private Vector2[] targetPositions = new Vector2[4];
    private Coroutine moveCoroutine;
    private bool isPatternActive = false;

    void Start()
    {
        // �ڷ�ƾ ���۸� �ϰ� ���� ������ isPatternActive�� ����
        moveCoroutine = StartCoroutine(MoveLoop());
    }

    public void SetActivePattern(bool active)
    {
        isPatternActive = active;

        if (!active)
        {
            // ���� ���� �� ������ ����
            for (int i = 0; i < magicCircles.Length; i++)
            {
                if (magicCircles[i] != null)
                {
                    Destroy(magicCircles[i].gameObject);
                    magicCircles[i] = null;
                }
            }
        }
        else
        {
            // ���� ���� �� ������ ����
            for (int i = 0; i < magicCircles.Length; i++)
            {
                if (magicCircles[i] == null)
                {
                    GameObject obj = Instantiate(magicCirclePrefab, startPositions[i], Quaternion.identity, this.transform);
                    magicCircles[i] = obj.transform;
                }
            }
            SetNextTargets();
        }
    }


    private void SetNextTargets()
    {
        targetPositions[0] = startPositions[1];
        targetPositions[1] = startPositions[2];
        targetPositions[2] = startPositions[3];
        targetPositions[3] = startPositions[0];
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            if (!isPatternActive)
            {
                yield return null;
                continue;
            }

            // magicCircles�� ��� �����ϴ��� Ȯ��
            bool anyNull = false;
            for (int i = 0; i < magicCircles.Length; i++)
            {
                if (magicCircles[i] == null)
                {
                    anyNull = true;
                    break;
                }
            }
            if (anyNull)
            {
                yield return null;
                continue;
            }

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
                if (!isPatternActive) break;

                t += Time.deltaTime / moveDuration;
                for (int i = 0; i < 4; i++)
                {
                    if (magicCircles[i] == null) continue;

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

            // Ÿ�� ��ġ�� ���� �̵� (���� Ȯ��)
            for (int i = 0; i < 4; i++)
            {
                if (magicCircles[i] != null)
                    magicCircles[i].position = targetPositions[i];
            }

            // ���� ��ġ ���
            Vector2[] newStartPositions = new Vector2[4];
            for (int i = 0; i < 4; i++) newStartPositions[i] = targetPositions[i];
            startPositions = newStartPositions;
            SetNextTargets();
        }
    }


    private void FireBullet(Vector3 spawnPos)
    {
        if (!isPatternActive || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (Vector2.zero - (Vector2)spawnPos).normalized;
            rb.velocity = dir * bulletSpeed;
        }
    }
}
