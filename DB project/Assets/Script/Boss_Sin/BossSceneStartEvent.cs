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
    public float bossAfterBgDelay = 0.5f;

    private PlayerController playerController;
    private GameObject currentPBoss;
    private bool isPlayingSequence = false;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player가 연결되지 않았습니다.");
            return;
        }

        // PlayerController 참조 및 이동 제한 해제
        playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.DisableAreaLimit(true);

        // Player 시작 위치
        player.position = startPoint.position;

        // 연출 코루틴 시작
        StartCoroutine(SceneStartSequence());
    }

    private IEnumerator SceneStartSequence()
    {
        isPlayingSequence = true;

        // 1. DummyB 이동
        if (dummyBPrefab != null)
        {
            GameObject dummy = Instantiate(dummyBPrefab, startPoint.position, Quaternion.identity);
            yield return MoveToPosition(dummy.transform, endPoint.position, dummySpeed);
            Destroy(dummy);
        }

        yield return new WaitForSecondsRealtime(playerDelay);

        // 2. Player 상승
        Vector3 playerTarget = player.position + Vector3.up * 2f;
        yield return MoveToPosition(player, playerTarget, playerMoveSpeed);

        yield return new WaitForSecondsRealtime(bossDelay);

        // 3. PBoss 등장
        if (pBossPrefab != null)
        {
            Vector3 bossStart = endPoint.position;
            Vector3 bossTarget = bossStart + Vector3.down * 4f;
            currentPBoss = Instantiate(pBossPrefab, bossStart, Quaternion.identity);
            yield return MoveToPosition(currentPBoss.transform, bossTarget, bossMoveSpeed);
        }

        // 4. 배경 전환 (한 프레임 대기 포함)
        BackgroundChanger bgChanger = FindObjectOfType<BackgroundChanger>();
        if (bgChanger != null)
        {
            bgChanger.ChangeToBossBackground();
            yield return null; // 한 프레임 대기 → GPU 부담 최소화
        }

        yield return new WaitForSecondsRealtime(bossAfterBgDelay);

        // 5. PBoss BGM 페이드 인 (BAudioManager 코루틴 내부에서 처리)
        BAudioManager.Instance?.PlayBossBGM();

        // 6. 연출 끝나면 Player 이동 제한 해제
        if (playerController != null)
            playerController.DisableAreaLimit(false);

        isPlayingSequence = false;
    }

    private IEnumerator MoveToPosition(Transform obj, Vector3 target, float speed)
    {
        while (Vector3.Distance(obj.position, target) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, speed * Time.unscaledDeltaTime);
            yield return null;
        }
        obj.position = target;
    }

    // PlayerController에서 호출 가능: 입력 차단 여부 확인
    public bool IsPlayingSequence() => isPlayingSequence;
}
