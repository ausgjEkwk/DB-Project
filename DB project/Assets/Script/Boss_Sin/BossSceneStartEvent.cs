using UnityEngine;
using System.Collections;

public class BossSceneStartEvent : MonoBehaviour
{
    [Header("����")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Prefab")]
    public GameObject dummyBPrefab;
    public GameObject pBossPrefab;
    public Transform player;

    [Header("�ӵ� ����")]
    public float dummySpeed = 10f;
    public float playerMoveSpeed = 5f;
    public float bossMoveSpeed = 6f;

    [Header("������ ����")]
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
            Debug.LogError("Player�� ������� �ʾҽ��ϴ�.");
            return;
        }

        // PlayerController ���� �� �̵� ���� ����
        playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.DisableAreaLimit(true);

        // Player ���� ��ġ
        player.position = startPoint.position;

        // ���� �ڷ�ƾ ����
        StartCoroutine(SceneStartSequence());
    }

    private IEnumerator SceneStartSequence()
    {
        isPlayingSequence = true;

        // 1. DummyB �̵�
        if (dummyBPrefab != null)
        {
            GameObject dummy = Instantiate(dummyBPrefab, startPoint.position, Quaternion.identity);
            yield return MoveToPosition(dummy.transform, endPoint.position, dummySpeed);
            Destroy(dummy);
        }

        yield return new WaitForSecondsRealtime(playerDelay);

        // 2. Player ���
        Vector3 playerTarget = player.position + Vector3.up * 2f;
        yield return MoveToPosition(player, playerTarget, playerMoveSpeed);

        yield return new WaitForSecondsRealtime(bossDelay);

        // 3. PBoss ����
        if (pBossPrefab != null)
        {
            Vector3 bossStart = endPoint.position;
            Vector3 bossTarget = bossStart + Vector3.down * 4f;
            currentPBoss = Instantiate(pBossPrefab, bossStart, Quaternion.identity);
            yield return MoveToPosition(currentPBoss.transform, bossTarget, bossMoveSpeed);
        }

        // 4. ��� ��ȯ (�� ������ ��� ����)
        BackgroundChanger bgChanger = FindObjectOfType<BackgroundChanger>();
        if (bgChanger != null)
        {
            bgChanger.ChangeToBossBackground();
            yield return null; // �� ������ ��� �� GPU �δ� �ּ�ȭ
        }

        yield return new WaitForSecondsRealtime(bossAfterBgDelay);

        // 5. PBoss BGM ���̵� �� (BAudioManager �ڷ�ƾ ���ο��� ó��)
        BAudioManager.Instance?.PlayBossBGM();

        // 6. ���� ������ Player �̵� ���� ����
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

    // PlayerController���� ȣ�� ����: �Է� ���� ���� Ȯ��
    public bool IsPlayingSequence() => isPlayingSequence;
}
