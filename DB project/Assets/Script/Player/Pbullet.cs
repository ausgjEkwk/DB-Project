using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pbullet : MonoBehaviour
{
    private Rigidbody2D rb;          // �Ѿ� Rigidbody2D ����
    private Vector2 savedVelocity;   // �ð����� �� �ӵ� ����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D ��������
        savedVelocity = rb.velocity;       // �ʱ� �ӵ� ����
    }

    void Update()
    {
        // �ð����� ������ ��
        if (TimeStop.Instance != null && TimeStop.Instance.IsTimeStopped)
        {
            if (rb.velocity != Vector2.zero) // ���� �̵� ���̸�
            {
                savedVelocity = rb.velocity; // ���� �ӵ� ����
                rb.velocity = Vector2.zero;  // �ӵ� 0���� ����
                rb.isKinematic = true;       // ���� ���� ����
            }
        }
        else // �ð����� ���� ��
        {
            if (rb.velocity == Vector2.zero) // ������ ������ ���¸�
            {
                rb.isKinematic = false;      // ���� ���� �ٽ� ����
                rb.velocity = savedVelocity; // ����� �ӵ��� �̵� �簳
            }
        }
    }
}
