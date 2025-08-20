using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public bool isPlayer;

    // 체력 0이 되어 죽었을 때 호출하는 이벤트
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;

        // 플레이어라면 시작할 때 UI 초기화
        if (isPlayer)
        {
            HealthUIManager.Instance?.InitializeHearts(maxHealth);
            HealthUIManager.Instance?.UpdateHearts(currentHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return; // 음수 대미지 무시

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (isPlayer)
        {
            // UI 업데이트
            HealthUIManager.Instance?.UpdateHearts(currentHealth);

            if (currentHealth <= 0)
            {
                Debug.Log("플레이어 사망");

                // 1) 스프라이트 회색 처리
                SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in srs) sr.color = Color.gray;

                // 2) 게임 정지
                Time.timeScale = 0f;

                // 3) Game Over UI 표시
                FindObjectOfType<GameOverUIManager>()?.ShowGameOverUI();

                // 4) 사망 BGM 재생
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayerDied();

                // 5) 이벤트 호출
                OnDeath?.Invoke();
            }

        }
        else
        {
            if (currentHealth <= 0)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    public void Heal(int amount)
    {
        if (!isPlayer) return;
        if (amount <= 0) return; // 음수 회복 무시

        int oldMaxHealth = maxHealth;

        // 최대 체력 증가
        maxHealth += amount;
        if (maxHealth > 10) maxHealth = 10;

        // 최대체력이 변했다면 UI 하트 재설정
        if (maxHealth != oldMaxHealth)
        {
            HealthUIManager.Instance?.InitializeHearts(maxHealth);
        }

        // 현재 체력 증가
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        // UI 업데이트
        HealthUIManager.Instance?.UpdateHearts(currentHealth);
    }
}
