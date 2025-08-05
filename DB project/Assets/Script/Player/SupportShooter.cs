using UnityEngine;

public class SupportShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    public Transform player;       // �÷��̾� ��ġ ����ٴϱ��
    public Vector3 followOffset;   // �÷��̾� ���� ��� ��ġ

    public float rotationSpeed = 360f;  // �ʴ� 360�� ȸ��

    void Awake()
    {
        if (player == null)
        {
            PlayerController playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                player = playerController.transform;
            }
        }

        // PlayerShooter�� �ڱ� �ڽ� ���
        PlayerShooter playerShooter = FindObjectOfType<PlayerShooter>();
        if (playerShooter != null)
        {
            playerShooter.RegisterSupportShooter(this);
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
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.up * bulletSpeed;
        }
    }
}