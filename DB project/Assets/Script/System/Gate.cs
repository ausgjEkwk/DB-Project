using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject swordPrefab;
    public float moveSpeed = 5f;

    private Vector3[] directions = new Vector3[]
{
    Quaternion.Euler(0,0,0) * Vector3.up,     // 0��
    Quaternion.Euler(0,0,45) * Vector3.up,    // 45��
    Quaternion.Euler(0,0,90) * Vector3.up,    // 90��
    Quaternion.Euler(0,0,135) * Vector3.up,   // 135��
    Quaternion.Euler(0,0,180) * Vector3.up,   // 180��
    Quaternion.Euler(0,0,225) * Vector3.up,   // 225��
    Quaternion.Euler(0,0,270) * Vector3.up,   // 270��
    Quaternion.Euler(0,0,315) * Vector3.up    // 315��
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
