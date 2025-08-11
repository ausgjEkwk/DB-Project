using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;             // �÷��̾� Transform (Inspector���� ����)
    public EnemySpawner enemySpawner;    // EnemySpawner ���� (Inspector����)
    public float appearDuration = 1.5f;  // ���忡 �ɸ��� �ð�

    void Start()
    {
        if (player != null)
        {
            // ���� �� ������ ��Ȱ��ȭ
            if (enemySpawner != null)
                enemySpawner.enabled = false;

            StartCoroutine(PlayerAppear());
        }
        else
        {
            Debug.LogWarning("�÷��̾ GameManager�� ������� �ʾҽ��ϴ�.");
        }
    }

    private IEnumerator PlayerAppear()
    {
        // ���� ��ġ: ȭ�� �� (0, -7, 0)
        Vector3 startPos = new Vector3(0f, -7f, 0f);
        // ��ǥ ��ġ: (0, -5, 0)
        Vector3 targetPos = new Vector3(0f, -5f, 0f);

        // ���� ���� �÷��̾� ��ġ�� startPos�� �̵�
        player.position = startPos;

        float elapsed = 0f;
        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / appearDuration;

            // �ε巴�� �̵�
            player.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // ��ġ ����
        player.position = targetPos;

        // 1�� ��� �� ������ Ȱ��ȭ
        yield return new WaitForSeconds(1f);

        if (enemySpawner != null)
            enemySpawner.enabled = true;
    }
}
