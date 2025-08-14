using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private Health playerHealth;

    void Start()
    {
        // �θ�(Player)�� ���� Health ã��
        playerHealth = GetComponentInParent<Health>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� źȯ�� �浹 ��
        if (collision.CompareTag("Bbullet") || collision.CompareTag("Rbullet") || collision.CompareTag("Ybullet") || collision.CompareTag("BossBullet"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            // źȯ ����
            Destroy(collision.gameObject);
        }
    }
}

