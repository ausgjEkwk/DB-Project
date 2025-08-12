using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern3 : MonoBehaviour
{
    public GameObject gatePrefab;              // Gate 프리팹 (Inspector에 연결)
    public float verticalSpacing = 2f;         // Gate 세로 간격
    public float horizontalDistance = 2f;      // 플레이어 좌우 위치까지 거리
    public int spawnCount = 9;                  // 세로 Gate 개수 (홀수 권장)
    public float bulletSpeed = 5f;              // 칼 이동 속도

    private Transform player;
    private List<GameObject> swords = new List<GameObject>();
    private List<GateController> spawnedGates = new List<GateController>(); // Gate 리스트
    private float moveSpeed;

    private BossSpecial bossSpecial;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        moveSpeed = bulletSpeed;

        bossSpecial = GetComponent<BossSpecial>();
        if (bossSpecial == null)
        {
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
        }
    }

    public void RegisterSword(GameObject sword)
    {
        swords.Add(sword);
    }

    public IEnumerator ExecutePattern()
    {
        // BossSpecial 실행 중이면 패턴 실행 안 함
        if (bossSpecial != null && bossSpecial.IsRunning)
        {
            yield break;
        }

        if (player == null)
        {
            Debug.LogWarning("Player 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        TimeStop.Instance?.StartTimeStop();

        Vector3 basePos = player.position;

        int half = spawnCount / 2;
        List<int> offsets = new List<int>();
        for (int i = -half; i <= half; i++)
        {
            offsets.Add(i);
        }

        // Gate 소환 (좌우 두개씩, 아래부터 위로, 0.5초 간격)
        foreach (int offset in offsets)
        {
            float yPos = basePos.y + offset * verticalSpacing;

            Vector3 leftPos = new Vector3(basePos.x - horizontalDistance, yPos, 0);
            Vector3 rightPos = new Vector3(basePos.x + horizontalDistance, yPos, 0);

            SpawnGateAtPosition(leftPos);
            SpawnGateAtPosition(rightPos);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        TimeStop.Instance?.EndTimeStop();

        // 시간 정지 해제 후 Gate에 칼 발사 시작 명령 보내기
        foreach (var gate in spawnedGates)
        {
            if (gate != null)
                gate.StartLaunch();
        }
        spawnedGates.Clear();

        // 칼 발사 (이동) 시작
        yield return MoveSwords();
    }

    private void SpawnGateAtPosition(Vector3 pos)
    {
        GameObject gateObj = Instantiate(gatePrefab, pos, Quaternion.identity);
        GateController gateCtrl = gateObj.GetComponent<GateController>();

        if (gateCtrl != null)
        {
            gateCtrl.bossPattern = this;
            spawnedGates.Add(gateCtrl);
        }
        else
        {
            Debug.LogWarning("Gate 프리팹에 GateController 컴포넌트가 없습니다.");
        }
    }

    private IEnumerator MoveSwords()
    {
        float moveDuration = 3f;
        float elapsed = 0f;

        Vector3[] directions = new Vector3[]
        {
            Quaternion.Euler(0,0,0) * Vector3.up,
            Quaternion.Euler(0,0,45) * Vector3.up,
            Quaternion.Euler(0,0,90) * Vector3.up,
            Quaternion.Euler(0,0,135) * Vector3.up,
            Quaternion.Euler(0,0,180) * Vector3.up,
            Quaternion.Euler(0,0,225) * Vector3.up,
            Quaternion.Euler(0,0,270) * Vector3.up,
            Quaternion.Euler(0,0,315) * Vector3.up
        };

        while (elapsed < moveDuration)
        {
            float delta = Time.deltaTime;
            elapsed += delta;

            for (int i = 0; i < swords.Count; i++)
            {
                GameObject sword = swords[i];
                if (sword == null) continue;

                int dirIndex = i % directions.Length;
                Vector3 dir = directions[dirIndex].normalized;

                sword.transform.position += dir * moveSpeed * delta;
            }

            yield return null;
        }

        foreach (var sword in swords)
        {
            if (sword != null)
                Destroy(sword);
        }
        swords.Clear();
    }
}
