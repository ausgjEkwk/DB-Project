using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject swordPrefab;
    public float moveSpeed = 5f;

    private Vector3[] directions = new Vector3[]
    {
        Vector3.up,
        Quaternion.Euler(0,0,60) * Vector3.up,
        Quaternion.Euler(0,0,120) * Vector3.up,
        Vector3.down,
        Quaternion.Euler(0,0,240) * Vector3.up,
        Quaternion.Euler(0,0,300) * Vector3.up
    };

    public BossPattern3 bossPattern; // 칼 리스트 등록용 (옵션)

    // BossPattern3가 호출해서 칼 발사 시작
    public void StartLaunch()
    {
        StartCoroutine(LaunchCoroutine());
    }

    private IEnumerator LaunchCoroutine()
    {
        foreach (var dir in directions)
        {
            GameObject sword = Instantiate(swordPrefab, transform.position, Quaternion.identity);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            sword.transform.rotation = Quaternion.Euler(0, 0, angle);

            Rigidbody2D rb2d = sword.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.bodyType = RigidbodyType2D.Kinematic;  // 유지
            }

            if (bossPattern != null)
            {
                bossPattern.RegisterSword(sword);
            }
        }

        // 칼 발사 시작과 동시에 Gate 오브젝트 삭제
        Destroy(gameObject);
        yield break;
    }
}
