using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;             // 플레이어 Transform (Inspector에서 연결)
    public EnemySpawner enemySpawner;    // EnemySpawner 연결 (Inspector에서)
    public float appearDuration = 1.5f;  // 등장에 걸리는 시간

    void Start()
    {
        if (player != null)
        {
            // 시작 시 스포너 비활성화
            if (enemySpawner != null)
                enemySpawner.enabled = false;

            StartCoroutine(PlayerAppear());
        }
        else
        {
            Debug.LogWarning("플레이어가 GameManager에 연결되지 않았습니다.");
        }
    }

    private IEnumerator PlayerAppear()
    {
        // 시작 위치: 화면 밖 (0, -7, 0)
        Vector3 startPos = new Vector3(0f, -7f, 0f);
        // 목표 위치: (0, -5, 0)
        Vector3 targetPos = new Vector3(0f, -5f, 0f);

        // 시작 전에 플레이어 위치를 startPos로 이동
        player.position = startPos;

        float elapsed = 0f;
        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / appearDuration;

            // 부드럽게 이동
            player.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // 위치 보정
        player.position = targetPos;

        // 1초 대기 후 스포너 활성화
        yield return new WaitForSeconds(1f);

        if (enemySpawner != null)
            enemySpawner.enabled = true;
    }
}
