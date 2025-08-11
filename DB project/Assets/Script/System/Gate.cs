using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject swordPrefab;
    public float moveSpeed = 5f;

    private Vector3[] directions = new Vector3[]
{
    Quaternion.Euler(0,0,0) * Vector3.up,     // 0도
    Quaternion.Euler(0,0,45) * Vector3.up,    // 45도
    Quaternion.Euler(0,0,90) * Vector3.up,    // 90도
    Quaternion.Euler(0,0,135) * Vector3.up,   // 135도
    Quaternion.Euler(0,0,180) * Vector3.up,   // 180도
    Quaternion.Euler(0,0,225) * Vector3.up,   // 225도
    Quaternion.Euler(0,0,270) * Vector3.up,   // 270도
    Quaternion.Euler(0,0,315) * Vector3.up    // 315도
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
