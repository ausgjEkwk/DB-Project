using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern3 : MonoBehaviour
{
    public GameObject gatePrefab; // 생성할 게이트 프리팹
    public float verticalSpacing = 2f; // 세로 간격
    public float horizontalDistance = 2f; // 플레이어 기준 좌우 거리
    public int spawnCount = 9; // 세로로 생성할 수량
    public float bulletSpeed = 5f; // 칼 이동 속도

    private Transform player; // 플레이어 위치
    private List<GameObject> swords = new List<GameObject>(); // 생성된 칼 리스트
    private List<GateController> spawnedGates = new List<GateController>(); // 생성된 게이트 리스트
    private float moveSpeed; // 칼 이동 속도
    private BossSpecial bossSpecial; // 특수패턴 실행 여부 확인

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform; // 플레이어 찾기
        moveSpeed = bulletSpeed; // 이동 속도 초기화

        bossSpecial = GetComponent<BossSpecial>(); // BossSpecial 참조
        if (bossSpecial == null)
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
    }

    public void RegisterSword(GameObject sword) // Gate에서 생성된 칼 등록
    {
        if (sword != null)
            sword.tag = "BossBullet"; // BossBullet 태그 부여

        swords.Add(sword); // 리스트에 추가
    }

    public IEnumerator ExecutePattern() // 패턴 실행 코루틴
    {
        if (player == null)
        {
            Debug.LogWarning("Player 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        TimeStop.Instance?.StartTimeStop(); // 시간 정지 시작

        Vector3 basePos = player.position; // 플레이어 기준
        int half = spawnCount / 2;
        List<int> offsets = new List<int>();
        for (int i = -half; i <= half; i++) // 세로 오프셋 계산
            offsets.Add(i);

        foreach (int offset in offsets)
        {
            float yPos = basePos.y + offset * verticalSpacing; // 세로 위치
            Vector3 leftPos = new Vector3(basePos.x - horizontalDistance, yPos, 0); // 좌측 위치
            Vector3 rightPos = new Vector3(basePos.x + horizontalDistance, yPos, 0); // 우측 위치

            SpawnGateAtPosition(leftPos); // 좌측 게이트 생성
            SpawnGateAtPosition(rightPos); // 우측 게이트 생성

            yield return new WaitUntil(() => Time.timeScale > 0f); // 시간 정지 해제 대기
            yield return new WaitForSeconds(0.5f); // 0.5초 간격
        }

        TimeStop.Instance?.EndTimeStop(); // 시간 정지 종료

        foreach (var gate in spawnedGates) // 모든 게이트 발사
            if (gate != null)
                gate.StartLaunch();

        spawnedGates.Clear(); // 리스트 초기화

        yield return MoveSwords(); // 칼 이동
    }

    private void SpawnGateAtPosition(Vector3 pos) // 위치에 게이트 생성
    {
        GameObject gateObj = Instantiate(gatePrefab, pos, Quaternion.identity); // 생성
        GateController gateCtrl = gateObj.GetComponent<GateController>(); // GateController 참조

        if (gateCtrl != null)
        {
            gateCtrl.bossPattern = this; // 참조 연결
            spawnedGates.Add(gateCtrl); // 리스트에 추가
        }
    }

    private IEnumerator MoveSwords() // 시간 정지 후 발사되는 칼 이동
    {
        float moveDuration = 3f; // 이동 시간
        float elapsed = 0f;

        Vector3[] directions = new Vector3[]
        {
            Quaternion.Euler(0,0,0) * Vector3.up, // 0도
            Quaternion.Euler(0,0,45) * Vector3.up, // 45도
            Quaternion.Euler(0,0,90) * Vector3.up, // 90도
            Quaternion.Euler(0,0,135) * Vector3.up, // 135도
            Quaternion.Euler(0,0,180) * Vector3.up, // 180도
            Quaternion.Euler(0,0,225) * Vector3.up, // 225도
            Quaternion.Euler(0,0,270) * Vector3.up, // 270도
            Quaternion.Euler(0,0,315) * Vector3.up // 315도
        };

        while (elapsed < moveDuration) // 이동 반복
        {
            float delta = Time.deltaTime;
            elapsed += delta;

            for (int i = 0; i < swords.Count; i++)
            {
                GameObject sword = swords[i];
                if (sword == null) continue;

                int dirIndex = i % directions.Length; // 방향 선택
                Vector3 dir = directions[dirIndex].normalized; // 단위 벡터

                sword.transform.position += dir * moveSpeed * delta; // 이동
            }

            yield return null;
        }

        foreach (var sword in swords) // 이동 종료 후 삭제
        {
            if (sword != null)
            {
                sword.tag = "BossBullet"; // 태그 재설정 (삭제 대상)
                Destroy(sword);
            }
        }
        swords.Clear(); // 리스트 초기화
    }
}
