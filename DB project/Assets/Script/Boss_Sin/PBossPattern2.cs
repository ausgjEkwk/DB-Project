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
    public float fireDistanceInterval = 0.1f; // Inspector���� ���� ����

    [Header("������ ���� ��ġ")]
    public Vector2[] loopPositions = new Vector2[4]
    {
        new Vector2(-3.3f, 6f),   // 0
        new Vector2(-3.3f, -6f),  // 1
        new Vector2(3.3f, -6f),   // 2
        new Vector2(3.3f, 6f)     // 3
    };

    [Header("�̵� �ӵ� ���� (1�ʴ� �̵�)")]
    public float moveDuration = 1.5f;

    private Transform[] magicCircles = new Transform[4];
    private int[] indices = new int[4]; // �� �������� �����ϴ� loopPositions �ε���

    void Start()
    {
        // ������ ���� & �ʱ� �ε��� ����
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
            // ��ǥ ��ġ ���
            Vector2[] targetPositions = new Vector2[4];
            for (int i = 0; i < 4; i++)
            {
                // 1,2�� �� �ð����
                if (i == 0 || i == 1)
                    targetPositions[i] = loopPositions[(indices[i] + 1) % loopPositions.Length];
                // 3,4�� �� �ݽð����
                else
                    targetPositions[i] = loopPositions[(indices[i] - 1 + loopPositions.Length) % loopPositions.Length];
            }

            // �̵� ����
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

            // ���� ����
            for (int i = 0; i < 4; i++)
                magicCircles[i].position = targetPositions[i];

            // �ε��� ����
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 1)
                    indices[i] = (indices[i] + 1) % loopPositions.Length; // �ð�
                else
                    indices[i] = (indices[i] - 1 + loopPositions.Length) % loopPositions.Length; // �ݽð�
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
