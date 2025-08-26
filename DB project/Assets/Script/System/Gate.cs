using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject swordPrefab; // 소환할 검 프리팹
    public float moveSpeed = 5f;   // (현재는 사용 안됨, 필요 시 Gate 이동용)

    // 8방향 벡터 (0,45,90,135,...315도)
    private Vector3[] directions = new Vector3[]
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

    public BossPattern3 bossPattern; // Gate에서 생성된 검을 BossPattern3에 등록

    // Gate에서 검 발사 시작
    public void StartLaunch()
    {
        StartCoroutine(LaunchCoroutine()); // 코루틴 시작
    }

    // 8방향으로 검을 생성하고 회전 후 BossPattern3에 등록
    private IEnumerator LaunchCoroutine()
    {
        foreach (var dir in directions)
        {
            // 검 프리팹 생성
            GameObject sword = Instantiate(swordPrefab, transform.position, Quaternion.identity);

            sword.tag = "BossBullet"; // 보스 총알 태그 지정

            // 벡터 방향에 맞춰 회전
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            sword.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Rigidbody2D를 Kinematic으로 설정 (직접 이동 제어용)
            Rigidbody2D rb2d = sword.GetComponent<Rigidbody2D>();
            if (rb2d != null)
                rb2d.bodyType = RigidbodyType2D.Kinematic;

            // BossPattern3에 검 등록
            bossPattern?.RegisterSword(sword);
        }

        Destroy(gameObject); // Gate 제거
        yield break;
    }
}
