using UnityEngine;

public class SupportShooter : MonoBehaviour
{
    public GameObject bulletPrefab;       // ����Ʈ �Ѿ� ������
    public Transform firePoint;           // �Ѿ� �߻� ��ġ
    public float bulletSpeed = 10f;       // �Ѿ� �ӵ�

    public Transform player;              // �÷��̾� ��ġ ����ٴϱ��
    public Vector3 followOffset;          // �÷��̾� ���� ��� ��ġ

    public float rotationSpeed = 360f;    // �ʴ� 360�� ȸ�� (1�ʿ� 1����)

    void Awake()
    {
        if (player == null)               // �÷��̾ �Ҵ�Ǿ� ���� ������
        {
            PlayerController playerController = FindObjectOfType<PlayerController>(); // ������ PlayerController �˻�
            if (playerController != null)
            {
                player = playerController.transform; // �÷��̾� Transform �Ҵ�
            }
        }

        // PlayerShooter�� �ڱ� �ڽ� ���
        PlayerShooter playerShooter = FindObjectOfType<PlayerShooter>();
        if (playerShooter != null)
        {
            playerShooter.RegisterSupportShooter(this); // �Ѿ� �߻� �� ����Ʈ�� �Բ� �߻�ǵ��� ���
        }
    }

    void Update()
    {
        if (player != null)
        {
            if (followOffset == Vector3.zero)
            {
                // ��ġ ����: Instantiate ���� ��ġ ���� (������ ����)
                // �ƹ��͵� �� �ص� ��
            }
            else
            {
                // �÷��̾� ��ġ ���� ��� ��ġ�� ����ٴ�
                transform.position = player.position + followOffset;
            }
        }

        // Z�� �������� ��� ȸ�� (1�ʿ� 1����)
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    public void Fire()
    {
        if (bulletPrefab != null && firePoint != null)   // �Ѿ˰� �߻� ��ġ�� �����ϸ�
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity); // �Ѿ� ����
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;       // �� �������� �߻�
        }
    }
}
