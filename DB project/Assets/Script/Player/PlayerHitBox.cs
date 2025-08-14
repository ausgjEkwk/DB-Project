using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private Health playerHealth;

    void Start()
    {
        // 부모(Player)에 붙은 Health 찾기
        playerHealth = GetComponentInParent<Health>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 몬스터 탄환과 충돌 시
        if (collision.CompareTag("Bbullet") || collision.CompareTag("Rbullet") || collision.CompareTag("Ybullet") || collision.CompareTag("BossBullet"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            // 탄환 제거
            Destroy(collision.gameObject);
        }
    }
}

