using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject swordPrefab; // ��ȯ�� �� ������
    public float moveSpeed = 5f;   // (����� ��� �ȵ�, �ʿ� �� Gate �̵���)

    // 8���� ���� (0,45,90,135,...315��)
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

    public BossPattern3 bossPattern; // Gate���� ������ ���� BossPattern3�� ���

    // Gate���� �� �߻� ����
    public void StartLaunch()
    {
        StartCoroutine(LaunchCoroutine()); // �ڷ�ƾ ����
    }

    // 8�������� ���� �����ϰ� ȸ�� �� BossPattern3�� ���
    private IEnumerator LaunchCoroutine()
    {
        foreach (var dir in directions)
        {
            // �� ������ ����
            GameObject sword = Instantiate(swordPrefab, transform.position, Quaternion.identity);

            sword.tag = "BossBullet"; // ���� �Ѿ� �±� ����

            // ���� ���⿡ ���� ȸ��
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            sword.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Rigidbody2D�� Kinematic���� ���� (���� �̵� �����)
            Rigidbody2D rb2d = sword.GetComponent<Rigidbody2D>();
            if (rb2d != null)
                rb2d.bodyType = RigidbodyType2D.Kinematic;

            // BossPattern3�� �� ���
            bossPattern?.RegisterSword(sword);
        }

        Destroy(gameObject); // Gate ����
        yield break;
    }
}
