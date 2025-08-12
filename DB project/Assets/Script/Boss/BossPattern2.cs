using System.Collections.Generic;
using UnityEngine;

public class BossPattern2 : MonoBehaviour
{
    public GameObject swordPrefab;
    public float radius = 1.5f;
    public int swordCount = 8;
    public float swordSpeed = 10f;
    public float spawnDistanceInterval = 1f;

    private List<GameObject> spawnedSwords = new List<GameObject>();
    private Vector3 lastSpawnPos;
    private Transform playerTransform;

    private BossSpecial bossSpecial;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogError("BossPattern2: 'Player' 태그 오브젝트를 찾을 수 없습니다.");

        bossSpecial = GetComponent<BossSpecial>();
        if (bossSpecial == null)
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
    }

    public void ResetPattern(Vector3 startPos)
    {
        lastSpawnPos = startPos;
        spawnedSwords.Clear();
    }

    public void UpdatePatternDuringMove()
    {
        // BossSpecial 실행 중이면 이 함수 아무것도 안 함
        if (bossSpecial != null && bossSpecial.IsRunning)
            return;

        if (playerTransform == null) return;

        if (Vector3.Distance(transform.position, lastSpawnPos) >= spawnDistanceInterval)
        {
            SpawnSwordsInCircle(transform.position);
            lastSpawnPos = transform.position;
        }
    }

    void SpawnSwordsInCircle(Vector3 centerPos)
    {
        for (int i = 0; i < swordCount; i++)
        {
            float angle = 360f / swordCount * i;
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;
            Vector3 spawnPos = centerPos + offset;

            Vector3 dirToPlayer = (playerTransform.position - spawnPos).normalized;
            float angleToPlayer = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angleToPlayer);

            GameObject sword = Instantiate(swordPrefab, spawnPos, rotation);
            spawnedSwords.Add(sword);

            SwordMover mover = sword.GetComponent<SwordMover>();
            if (mover != null)
            {
                mover.SetDirection(dirToPlayer, swordSpeed);
                mover.PauseMovement();  // 이동 일시정지 상태로 시작
            }
        }
    }

    public void ShootAllSwords()
    {
        // BossSpecial 실행 중일 경우 절대 발사하지 않음
        if (bossSpecial != null && bossSpecial.IsRunning)
            return;

        foreach (GameObject sword in spawnedSwords)
        {
            if (sword != null)
            {
                SwordMover mover = sword.GetComponent<SwordMover>();
                if (mover != null)
                {
                    mover.ResumeMovement();
                }
            }
        }
    }
}
