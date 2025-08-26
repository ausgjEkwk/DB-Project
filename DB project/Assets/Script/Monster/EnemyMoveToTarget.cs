using System;
using UnityEngine;

public class EnemyMoveToTarget : MonoBehaviour
{
    public Vector2 targetPosition;     // 목표 위치
    public float moveSpeed = 2f;       // 이동 속도
    public GameObject itemPrefab;      // 사망 시 생성할 아이템 프리팹

    public event Action OnReachedTargetEvent; // 목표 도달 시 발생하는 이벤트

    private bool hasReachedTarget = false;   // 목표 도달 여부
    private bool isDestroyed = false;        // 이미 제거되었는지 여부

    private Health health;                   // Health 컴포넌트 참조

    void Awake()
    {
        health = GetComponent<Health>();     // Health 컴포넌트 가져오기
        if (health != null)
        {
            health.OnDeath += Die;           // Health의 사망 이벤트에 Die() 연결
        }
    }

    void Update()
    {
        if (hasReachedTarget || isDestroyed) return; // 이미 도달했거나 제거되었으면 이동하지 않음

        // 목표 위치로 이동
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 목표 근접 시
        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            hasReachedTarget = true;                 // 도달 플래그 설정
            OnReachedTargetEvent?.Invoke();          // 도달 이벤트 호출
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // 몬스터 탄환 태그 목록
        string[] monsterBulletTags = { "Rbullet", "Bbullet", "Ybullet" };

        // 몬스터 탄환이면 무시
        foreach (var tag in monsterBulletTags)
        {
            if (collision.CompareTag(tag))
            {
                return; // 몬스터 총알 충돌 무시
            }
        }

        // 플레이어 총알 충돌 처리
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);         // 총알 제거

            if (health != null)
            {
                health.TakeDamage(1);              // 체력 1 감소
            }
            else
            {
                Die();                              // Health 없으면 즉시 사망
            }
        }
    }

    // 폭탄(Boom)에 의해 즉시 제거
    public void DestroyByBoom()
    {
        if (isDestroyed) return;

        if (health != null)
        {
            health.TakeDamage(health.currentHealth); // 체력을 0으로 만들어 사망 처리
        }
        else
        {
            Die();                                   // Health 없으면 즉시 사망
        }
    }

    // 몬스터 사망 처리
    private void Die()
    {
        if (isDestroyed) return;                    // 이미 제거되었으면 처리 안 함
        isDestroyed = true;

        // 아이템 생성
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);                        // 몬스터 오브젝트 제거
    }
}
