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

    public BossPattern3 bossPattern; // Į ����Ʈ ��Ͽ� (�ɼ�)

    // BossPattern3�� ȣ���ؼ� Į �߻� ����
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
                rb2d.bodyType = RigidbodyType2D.Kinematic;  // ����
            }

            if (bossPattern != null)
            {
                bossPattern.RegisterSword(sword);
            }
        }

        // Į �߻� ���۰� ���ÿ� Gate ������Ʈ ����
        Destroy(gameObject);
        yield break;
    }
}
