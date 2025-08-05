using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bossPrefab;            // 보스 프리팹

    public GameObject[] monsterPrefabs;      // 몬스터 그룹 프리팹 배열 (A, B, C)
    public float spawnInterval = 2f;         // 몬스터 생성 간격 (초)

    private BoxCollider2D spawnArea;         // 몬스터 스폰 영역

    // --- 아이템 관련 변수 ---
    public GameObject itemPrefab;
    public float itemSpawnInterval = 10f;
    public float itemFallSpeed = 2f;

    private float itemSpawnTimer = 0f;

    

    private int waveCount = 0;
    private bool bossSpawned = false;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        StartCoroutine(SpawnMonsters());
    }

    void Update()
    {
        // 아이템 생성 타이머
        itemSpawnTimer += Time.deltaTime;
        if (itemSpawnTimer >= itemSpawnInterval)
        {
            itemSpawnTimer = 0f;
            SpawnItem();
        }
    }

    IEnumerator SpawnMonsters()
    {
        while (waveCount < 5)
        {
            for (int i = 0; i < monsterPrefabs.Length; i++)  // A, B, C 그룹 순서대로
            {
                Vector2 spawnPos = GetRandomPositionInBox();
                GameObject monsterGroup = Instantiate(monsterPrefabs[i], spawnPos, Quaternion.identity);

                // 그룹 자식 몬스터 각각에 EnemyMoveToTarget & MonsterShooter 연결
                foreach (Transform child in monsterGroup.transform)
                {
                    EnemyMoveToTarget moveScript = child.GetComponent<EnemyMoveToTarget>();
                    MonsterShooter shooter = child.GetComponent<MonsterShooter>();

                    if (moveScript != null)
                    {
                        Vector2 childWorldPos = child.position;
                        moveScript.targetPosition = childWorldPos + Vector2.down * 3f;

                        if (shooter != null)
                            moveScript.OnReachedTargetEvent += shooter.StartShooting;
                    }
                }

                yield return new WaitForSeconds(spawnInterval);
            }

            waveCount++;
        }

        // 5웨이브 끝나면 보스 소환 조건 체크 시작
        StartCoroutine(CheckAndSpawnBoss());
    }

    IEnumerator CheckAndSpawnBoss()
    {
        // 몬스터가 모두 제거될 때까지 대기
        while (GameObject.FindGameObjectsWithTag("Monster").Length > 0)
        {
            yield return null;
        }

        // 5초 대기 후 보스 소환
        yield return new WaitForSeconds(5f);

        if (!bossSpawned)
        {
            SpawnBoss();
            bossSpawned = true;
        }
    }

    void SpawnBoss()
    {
        Vector3 spawnPos = transform.position; // EnemySpawner의 중심에서 바로 소환

        GameObject bossObj = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        BossMovement bossMovement = bossObj.GetComponent<BossMovement>();
        if (bossMovement != null)
        {
            bossMovement.StartMovePattern(); // 시작 위치에서 직접 내려가게 함
        }
        else
        {
            Debug.LogError("보스 프리팹에 BossMovement 컴포넌트 없음");
        }
    }



    Vector2 GetRandomPositionInBox()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x + 0.7f, bounds.max.x - 0.7f);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    void SpawnItem()
    {
        Vector2 spawnPos2D = GetRandomPositionInBox();
        Vector3 spawnPos = new Vector3(spawnPos2D.x, spawnPos2D.y, 0f);
        GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        FallingItem fallingItem = item.GetComponent<FallingItem>();
        if (fallingItem != null)
        {
            fallingItem.fallSpeed = itemFallSpeed;
        }
    }
}
