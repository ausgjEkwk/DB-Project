using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject bossPrefab;            // 소환할 보스 프리팹
    public GameObject[] monsterPrefabs;      // 몬스터 그룹 프리팹 배열 (A, B, C)
    public float spawnInterval = 2f;         // 몬스터 그룹 소환 간격(초)

    private BoxCollider2D spawnArea;         // 몬스터 스폰 영역

    // --- 아이템 관련 변수 ---
    public GameObject itemPrefab;            // 떨어질 아이템 프리팹
    public float itemSpawnInterval = 10f;    // 아이템 생성 간격(초)
    public float itemFallSpeed = 2f;         // 아이템 낙하 속도

    private float itemSpawnTimer = 0f;       // 아이템 타이머
    private int waveCount = 0;               // 현재 웨이브 수
    private bool bossSpawned = false;        // 보스 소환 여부

    void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();  // 스폰 영역 콜라이더 가져오기
        StartCoroutine(SpawnMonsters());           // 몬스터 소환 코루틴 시작
    }

    void Update()
    {
        // 아이템 생성 타이머 갱신
        itemSpawnTimer += Time.deltaTime;
        if (itemSpawnTimer >= itemSpawnInterval)
        {
            itemSpawnTimer = 0f;
            SpawnItem();                          // 아이템 생성
        }
    }

    IEnumerator SpawnMonsters()
    {
        while (waveCount < 6)                        // 총 6웨이브 진행
        {
            for (int i = 0; i < monsterPrefabs.Length; i++)  // A, B, C 그룹 순서대로
            {
                Vector2 spawnPos = GetRandomPositionInBox();            // 랜덤 위치 결정
                GameObject monsterGroup = Instantiate(monsterPrefabs[i], spawnPos, Quaternion.identity); // 몬스터 생성

                // 그룹 자식 몬스터 각각에 EnemyMoveToTarget & MonsterShooter 연결
                foreach (Transform child in monsterGroup.transform)
                {
                    EnemyMoveToTarget moveScript = child.GetComponent<EnemyMoveToTarget>();
                    MonsterShooter shooter = child.GetComponent<MonsterShooter>();

                    if (moveScript != null)
                    {
                        Vector2 childWorldPos = child.position;
                        moveScript.targetPosition = childWorldPos + Vector2.down * 3f; // 아래로 목표 설정

                        if (shooter != null)
                            moveScript.OnReachedTargetEvent += shooter.StartShooting; // 목표 도달 시 공격 시작
                    }
                }

                yield return new WaitForSeconds(spawnInterval); // 다음 몬스터 그룹까지 대기
            }

            waveCount++; // 웨이브 증가
        }

        // 웨이브 종료 후 몬스터가 모두 제거될 때까지 대기 후 BGM 페이드 아웃
        StartCoroutine(WaitForAllMonstersAndFadeBGM());
    }

    IEnumerator WaitForAllMonstersAndFadeBGM()
    {
        // 몬스터가 남아있는 동안 대기
        while (GameObject.FindGameObjectsWithTag("Monster").Length > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f); // 3초 대기

        // 모든 몬스터 제거 시 Normal BGM 페이드 아웃
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGMWithFade();
        }

        yield return new WaitForSeconds(3.5f); // 5초 후 보스 소환

        if (!bossSpawned)
        {
            SpawnBoss();            // 보스 소환
            bossSpawned = true;

            // 보스 BGM 재생
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBossBGM();
            }
        }
    }

    void SpawnBoss()
    {
        Vector3 spawnPos = transform.position; // EnemySpawner 위치에서 보스 소환
        GameObject bossObj = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        BossMovement bossMovement = bossObj.GetComponent<BossMovement>();
        if (bossMovement != null)
        {
            bossMovement.StartMovePattern(); // 보스 이동 패턴 시작
        }
        else
        {
            Debug.LogError("보스 프리팹에 BossMovement 컴포넌트 없음");
        }
    }

    // BoxCollider 영역 내 랜덤 위치 반환
    Vector2 GetRandomPositionInBox()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x + 0.7f, bounds.max.x - 0.7f); // 좌우 여유 0.7
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    void SpawnItem()
    {
        Vector2 spawnPos2D = GetRandomPositionInBox();            // 랜덤 위치 결정
        Vector3 spawnPos = new Vector3(spawnPos2D.x, spawnPos2D.y, 0f);
        GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity); // 아이템 생성

        FallingItem fallingItem = item.GetComponent<FallingItem>();
        if (fallingItem != null)
        {
            fallingItem.fallSpeed = itemFallSpeed; // 낙하 속도 적용
        }
    }
}
