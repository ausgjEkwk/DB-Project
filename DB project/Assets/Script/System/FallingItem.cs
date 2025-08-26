using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 2f; // ������ ���� �ӵ�
    private Camera mainCamera;    // ���� ī�޶� ����

    void Start()
    {
        mainCamera = Camera.main; // ������ ���� ī�޶� ��������
    }

    void Update()
    {
        // �ð� ���� ���̸� ������ �������� ����
        if (TimeStop.IsStopped)
            return;

        // �������� �Ʒ��� �̵�
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // ȭ�� ��ǥ�� ��ȯ
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

        // ȭ�� �Ʒ��� ����� ����
        if (viewPos.y < 0)
        {
            Destroy(gameObject);
        }
    }

    // �÷��̾�� �浹���� ��
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾� ü�� ȸ��
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(1);
            }
            // ������ ����
            Destroy(gameObject);
        }
    }
}
