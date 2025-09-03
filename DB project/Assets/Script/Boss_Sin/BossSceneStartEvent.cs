using UnityEngine;
using System.Collections;

public class BossSceneStartEvent : MonoBehaviour
{
    [Header("지점")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Prefab")]
    public GameObject dummyBPrefab;
    public GameObject pBossPrefab;
    public Transform player;

    [Header("속도 설정")]
    public float dummySpeed = 10f;
    public float playerMoveSpeed = 5f;
    public float bossMoveSpeed = 6f;

    [Header("딜레이 설정")]
    public float playerDelay = 1f;
    public float bossDelay = 1f;
    public float bossAfterBgDelay = 1f; // PBoss 등장 후 배경 전환 딜레이

    private PlayerController playerController;
    private GameObject currentPBoss;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player가 연결되지 않았습니다.");
            return;
        }

        // PlayerController 참조
        playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.DisableAreaLimit(true); // 연출 중 이동 제한 해제

        // Player 시작 위치
        player.position = startPoint.position;

        // 연출 코루틴 시작
        StartCoroutine(SceneStartSequence());
    }

    private IEnumerator SceneStartSequence()
    {
        // 1. Dumy_B 생성 후 End까지 이동
        if (dummyBPrefab != null)
        {
            GameObject dummy = Instantiate(dummyBPrefab, startPoint.position, Quaternion.identity);
            yield return StartCoroutine(MoveToPosition(dummy.transform, endPoint.position, dummySpeed));
            Destroy(dummy);
        }

        // 2. Dumy_B 삭제 후 1초 대기
        yield return new WaitForSeconds(playerDelay);

        // 3. Player 상승
        Vector3 playerTarget = player.position + new Vector3(0f, 2f, 0f);
        yield return StartCoroutine(MoveToPosition(player, playerTarget, playerMoveSpeed));

        // 4. Player 상승 완료 후 1초 대기
        yield return new WaitForSeconds(bossDelay);

        // 5. PBoss 등장
        if (pBossPrefab != null)
        {
            Vector3 bossStartPos = endPoint.position;
            Vector3 bossTarget = bossStartPos + new Vector3(0f, -4f, 0f);
            currentPBoss = Instantiate(pBossPrefab, bossStartPos, Quaternion.identity);
            yield return StartCoroutine(MoveToPosition(currentPBoss.transform, bossTarget, bossMoveSpeed));
        }

        // 6. PBoss 등장 후 1초 대기 → 배경 전환
        yield return new WaitForSeconds(bossAfterBgDelay);
        BackgroundChanger bgChanger = FindObjectOfType<BackgroundChanger>();
        if (bgChanger != null)
            bgChanger.ChangeToBossBackground();

        // 7. 연출 끝나면 Player 이동 제한 다시 활성화
        if (playerController != null)
            playerController.DisableAreaLimit(false);
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 target, float speed)
    {
        while (Vector3.Distance(obj.position, target) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, speed * Time.deltaTime);
            yield return null;
        }
        obj.position = target;
    }
}
