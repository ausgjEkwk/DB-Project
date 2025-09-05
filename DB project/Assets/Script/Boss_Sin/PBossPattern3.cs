using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBossPattern3 : MonoBehaviour
{
    [Header("탄환 프리팹")]
    public GameObject bulletPrefab;
    public float scatterSpeed = 5f;
    public float spawnInterval = 0.1f;

    [Header("가로줄 설정")]
    public float horizontalStartX = 3.5f;
    public float horizontalEndX = -3.5f;
    public float[] horizontalYLines = new float[] { 5f, 2f, -2f, -5f };
    public int horizontalBulletsPerLine = 15;

    [Header("세로줄 설정")]
    public float verticalStartY = 6f;
    public float verticalEndY = -6f;
    public float[] verticalXLines = new float[] { -2f, 0f, 2f };
    public int verticalBulletsPerLine = 12;

    private List<GameObject> allBullets = new List<GameObject>();
    private Coroutine patternCoroutine;
    private bool isPatternActive = false;

    public bool IsPatternFinished { get; private set; } = true;

    public void SetActivePattern(bool active)
    {
        isPatternActive = active;

        if (active && patternCoroutine == null)
        {
            patternCoroutine = StartCoroutine(SpawnPatternOnce());
        }
    }

    private IEnumerator SpawnPatternOnce()
    {
        IsPatternFinished = false;
        allBullets.Clear();

        int maxLines = Mathf.Max(horizontalYLines.Length, verticalXLines.Length);

        for (int i = 0; i < maxLines; i++)
        {
            Coroutine hor = null;
            Coroutine ver = null;

            if (i < horizontalYLines.Length)
                hor = StartCoroutine(SpawnHorizontalLine(horizontalYLines[i]));

            if (i < verticalXLines.Length)
                ver = StartCoroutine(SpawnVerticalLine(verticalXLines[i]));

            // 가로, 세로 한 줄씩 순차 생성
            if (hor != null) yield return hor;
            if (ver != null) yield return ver;
        }

        // 생성 완료 후 잠시 대기
        yield return new WaitForSeconds(0.2f);

        // 모든 탄환 랜덤 방향으로 이동
        foreach (var bullet in allBullets)
        {
            if (bullet == null) continue;
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                rb.velocity = dir * scatterSpeed;
            }
        }

        // 탄환이 날아가는 동안도 유지
        yield return new WaitForSeconds(1.5f);

        // 이동 완료 후 리스트 초기화
        allBullets.Clear();

        IsPatternFinished = true;
        patternCoroutine = null;
    }

    private IEnumerator SpawnHorizontalLine(float y)
    {
        for (int i = 0; i < horizontalBulletsPerLine; i++)
        {
            if (!isPatternActive) yield break;

            float t = (float)i / (horizontalBulletsPerLine - 1);
            float x = Mathf.Lerp(horizontalStartX, horizontalEndX, t);
            SpawnBullet(new Vector2(x, y));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator SpawnVerticalLine(float x)
    {
        for (int i = 0; i < verticalBulletsPerLine; i++)
        {
            if (!isPatternActive) yield break;

            float t = (float)i / (verticalBulletsPerLine - 1);
            float y = Mathf.Lerp(verticalStartY, verticalEndY, t);
            SpawnBullet(new Vector2(x, y));
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnBullet(Vector2 position)
    {
        if (!isPatternActive || bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        allBullets.Add(bullet);
    }
}
