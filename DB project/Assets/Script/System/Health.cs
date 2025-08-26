using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;       // 최대 체력
    public int currentHealth;       // 현재 체력
    public bool isPlayer;           // 플레이어인지 여부

    // 체력 0이 되어 죽었을 때 호출되는 이벤트 (외부 구독 가능)
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth; // 시작 시 체력 초기화

        if (isPlayer)
        {
            // 플레이어라면 UI 초기화 및 체력 표시
            HealthUIManager.Instance?.InitializeHearts(maxHealth);
            HealthUIManager.Instance?.UpdateHearts(currentHealth);
        }
    }

    // 피해 처리
    public void TakeDamage(int amount)
    {
        if (amount <= 0) return; // 음수 피해 무시

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0; // 음수 체력 방지

        if (isPlayer)
        {
            // UI 업데이트
            HealthUIManager.Instance?.UpdateHearts(currentHealth);

            // 체력 0 이하 시 플레이어 사망 처리
            if (currentHealth <= 0)
            {
                Debug.Log("플레이어 사망");

                // 1) 스프라이트 회색 처리 (죽은 효과)
                SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in srs)
                    sr.color = Color.gray;

                // 2) 게임 시간 정지
                Time.timeScale = 0f;

                // 3) Game Over UI 표시
                FindObjectOfType<GameOverUIManager>()?.ShowGameOverUI();

                // 4) 사망 BGM 재생
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayerDied();

                // 5) 외부 이벤트 호출
                OnDeath?.Invoke();
            }
        }
        else
        {
            // 적 또는 NPC 체력 0 이하 시
            if (currentHealth <= 0)
            {
                OnDeath?.Invoke(); // 이벤트 호출
                Destroy(gameObject); // 오브젝트 제거
            }
        }
    }

    // 체력 회복 처리
    public void Heal(int amount)
    {
        if (!isPlayer) return;    // 플레이어만 회복 가능
        if (amount <= 0) return;  // 음수 회복 무시

        int oldMaxHealth = maxHealth;

        // 최대 체력 증가 (최대 10)
        maxHealth += amount;
        if (maxHealth > 10) maxHealth = 10;

        // 최대 체력이 바뀌면 UI 하트 재설정
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
