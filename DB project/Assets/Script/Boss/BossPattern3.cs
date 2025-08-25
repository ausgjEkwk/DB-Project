using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern3 : MonoBehaviour
{
    public GameObject gatePrefab;
    public float verticalSpacing = 2f;
    public float horizontalDistance = 2f;
    public int spawnCount = 9;
    public float bulletSpeed = 5f;

    private Transform player;
    private List<GameObject> swords = new List<GameObject>();
    private List<GateController> spawnedGates = new List<GateController>();
    private float moveSpeed;
    private BossSpecial bossSpecial;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        moveSpeed = bulletSpeed;

        bossSpecial = GetComponent<BossSpecial>();
        if (bossSpecial == null)
            Debug.LogWarning("BossSpecial 컴포넌트가 없습니다. 특수패턴 실행 체크 불가.");
    }

    public void RegisterSword(GameObject sword)
    {
        // Gate에서 생성된 칼도 BossBullet 태그 붙이기
        if (sword != null)
            sword.tag = "BossBullet";

        swords.Add(sword);
    }

    public IEnumerator ExecutePattern()
    {
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
            offsets.Add(i);

        foreach (int offset in offsets)
        {
            float yPos = basePos.y + offset * verticalSpacing;
            Vector3 leftPos = new Vector3(basePos.x - horizontalDistance, yPos, 0);
            Vector3 rightPos = new Vector3(basePos.x + horizontalDistance, yPos, 0);

            SpawnGateAtPosition(leftPos);
            SpawnGateAtPosition(rightPos);

            yield return new WaitUntil(() => Time.timeScale > 0f);
            yield return new WaitForSeconds(0.5f);
        }

        TimeStop.Instance?.EndTimeStop();

        foreach (var gate in spawnedGates)
            if (gate != null)
                gate.StartLaunch();

        spawnedGates.Clear();

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
            {
                sword.tag = "BossBullet"; // 삭제 대상 태그
                Destroy(sword);
            }
        }
        swords.Clear();
    }
}
