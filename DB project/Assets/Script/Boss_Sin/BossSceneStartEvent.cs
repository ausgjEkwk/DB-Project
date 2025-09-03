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
    public float bossAfterBgDelay = 1f; // PBoss ���� �� ��� ��ȯ ������

    private PlayerController playerController;
    private GameObject currentPBoss;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player�� ������� �ʾҽ��ϴ�.");
            return;
        }

        // PlayerController ����
        playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.DisableAreaLimit(true); // ���� �� �̵� ���� ����

        // Player ���� ��ġ
        player.position = startPoint.position;

        // ���� �ڷ�ƾ ����
        StartCoroutine(SceneStartSequence());
    }

    private IEnumerator SceneStartSequence()
    {
        // 1. Dumy_B ���� �� End���� �̵�
        if (dummyBPrefab != null)
        {
            GameObject dummy = Instantiate(dummyBPrefab, startPoint.position, Quaternion.identity);
            yield return StartCoroutine(MoveToPosition(dummy.transform, endPoint.position, dummySpeed));
            Destroy(dummy);
        }

        // 2. Dumy_B ���� �� 1�� ���
        yield return new WaitForSeconds(playerDelay);

        // 3. Player ���
        Vector3 playerTarget = player.position + new Vector3(0f, 2f, 0f);
        yield return StartCoroutine(MoveToPosition(player, playerTarget, playerMoveSpeed));

        // 4. Player ��� �Ϸ� �� 1�� ���
        yield return new WaitForSeconds(bossDelay);

        // 5. PBoss ����
        if (pBossPrefab != null)
        {
            Vector3 bossStartPos = endPoint.position;
            Vector3 bossTarget = bossStartPos + new Vector3(0f, -4f, 0f);
            currentPBoss = Instantiate(pBossPrefab, bossStartPos, Quaternion.identity);
            yield return StartCoroutine(MoveToPosition(currentPBoss.transform, bossTarget, bossMoveSpeed));
        }

        // 6. PBoss ���� �� 1�� ��� �� ��� ��ȯ
        yield return new WaitForSeconds(bossAfterBgDelay);
        BackgroundChanger bgChanger = FindObjectOfType<BackgroundChanger>();
        if (bgChanger != null)
            bgChanger.ChangeToBossBackground();

        // 7. ���� ������ Player �̵� ���� �ٽ� Ȱ��ȭ
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
