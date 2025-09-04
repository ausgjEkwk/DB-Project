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
    private PlayerShooter playerShooter;
    private GameObject currentPBoss;
    private GameObject currentDummyB;
    private bool isPlayingSequence = false;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player가 연결되지 않았습니다.");
            return;
        }

        playerController = player.GetComponent<PlayerController>();
        playerShooter = player.GetComponent<PlayerShooter>();

        if (playerController != null)
        {
            playerController.DisableAreaLimit(true);
            playerController.SetInputLock(true);
        }

        if (playerShooter != null)
            playerShooter.SetShootLock(true);

        player.position = startPoint.position;

        // DummyB 생성
        if (dummyBPrefab != null)
        {
            currentDummyB = Instantiate(dummyBPrefab, startPoint.position, Quaternion.identity);
        }

        // DummyB 생성 후 1초 뒤 컷씬 시작
        StartCoroutine(DelayedStartSequence(1f));
    }

    private IEnumerator DelayedStartSequence(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        StartCoroutine(SceneStartSequence());
    }

    private IEnumerator SceneStartSequence()
    {
        isPlayingSequence = true;

        if (playerController != null)
            playerController.SetInputLock(true);

        // 1. DummyB 이동
        if (currentDummyB != null)
        {
            yield return MoveToPosition(currentDummyB.transform, endPoint.position, dummySpeed);
            Destroy(currentDummyB);
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

        // PBoss 도착 후 0.5초 대기
        yield return new WaitForSecondsRealtime(0.5f);

        // 4. 배경 전환 및 BGM 페이드 인
        BackgroundChanger bgChanger = FindObjectOfType<BackgroundChanger>();
        if (bgChanger != null)
        {
            bgChanger.ChangeToBossBackground();
        }

        BAudioManager.Instance?.PlayBossBGM();

        // 5. 연출 끝 → 이동 제한/입력 잠금 해제
        if (playerController != null)
        {
            playerController.DisableAreaLimit(false);
            playerController.SetInputLock(false);
        }
        if (playerShooter != null)
            playerShooter.SetShootLock(false);

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

    public bool IsPlayingSequence() => isPlayingSequence;
}
