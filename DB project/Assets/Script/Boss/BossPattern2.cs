using System.Collections.Generic; // List 사용
using UnityEngine;

public class BossPattern2 : MonoBehaviour
{
    public GameObject swordPrefab; // 생성할 검 프리팹
    public float radius = 1.5f; // 원형 배치 반지름
    public int swordCount = 8; // 한 번에 생성할 검 개수
    public float swordSpeed = 10f; // 검 이동 속도
    public float spawnDistanceInterval = 1f; // 이동 시 생성 간격

    private List<GameObject> spawnedSwords = new List<GameObject>(); // 생성된 검 리스트
    private Vector3 lastSpawnPos; // 마지막 생성 위치
    private Transform playerTransform; // 플레이어 위치 참조

    private BossSpecial bossSpecial; // 특수패턴 실행 여부 확인

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player"); // Player 태그 찾기
        if (player != null)
            playerTransform = player.transform; // 위치 저장
        else
            Debug.LogError("BossPattern2: 'Player' 태그 오브젝트를 찾을 수 없습니다.");

        bossSpecial = GetComponent<BossSpecial>(); // BossSpecial 참조
        if (bossSpecial == null)
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
    }

    public void ResetPattern(Vector3 startPos) // 패턴 초기화
    {
        lastSpawnPos = startPos; // 시작 위치 저장
        spawnedSwords.Clear(); // 기존 검 리스트 초기화
    }

    public void UpdatePatternDuringMove() // 보스 이동 중 검 생성
    {
        if (bossSpecial != null && bossSpecial.IsRunning) // 특수패턴 중이면 생성하지 않음
            return;

        if (playerTransform == null) return; // 플레이어 없으면 종료

        if (Vector3.Distance(transform.position, lastSpawnPos) >= spawnDistanceInterval) // 이동 간격 체크
        {
            SpawnSwordsInCircle(transform.position); // 원형으로 검 생성
            lastSpawnPos = transform.position; // 마지막 위치 갱신
        }
    }

    void SpawnSwordsInCircle(Vector3 centerPos) // 원형으로 검 생성
    {
        for (int i = 0; i < swordCount; i++)
        {
            float angle = 360f / swordCount * i; // 각도 계산
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius; // 위치 오프셋
            Vector3 spawnPos = centerPos + offset; // 최종 생성 위치

            Vector3 dirToPlayer = (playerTransform.position - spawnPos).normalized; // 플레이어 방향 계산
            float angleToPlayer = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg - 90f; // 회전 각도
            Quaternion rotation = Quaternion.Euler(0f, 0f, angleToPlayer); // 회전 설정

            GameObject sword = Instantiate(swordPrefab, spawnPos, rotation); // 검 생성
            spawnedSwords.Add(sword); // 리스트에 추가

            SwordMover mover = sword.GetComponent<SwordMover>(); // SwordMover 참조
            if (mover != null)
            {
                mover.SetDirection(dirToPlayer, swordSpeed); // 방향과 속도 설정
                mover.PauseMovement();  // 이동 일시정지 상태로 시작
            }
        }
    }

    public void ShootAllSwords() // 시간 정지 해제 후 검 발사
    {
        foreach (GameObject sword in spawnedSwords)
        {
            if (sword != null)
            {
                SwordMover mover = sword.GetComponent<SwordMover>(); // SwordMover 참조
                if (mover != null)
                {
                    mover.ResumeMovement(); // 이동 재개
                }
            }
        }
    }
}
