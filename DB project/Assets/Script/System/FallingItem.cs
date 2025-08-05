using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public float fallSpeed = 2f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // �ð� ���� ���̸� �������� ����
        if (TimeStop.IsStopped)
            return;

        // �Ʒ��� �̵�
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // ȭ�� �Ʒ��� ����� ����
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewPos.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(1);
            }
            Destroy(gameObject);
        }
    }
}
