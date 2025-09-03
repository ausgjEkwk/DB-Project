using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;       // 최대 체력
    public int currentHealth;       // 현재 체력
    public bool isPlayer;           // 플레이어인지 여부
    public bool isCenterMonster;    // 그룹의 중앙 몬스터인지 여부 (Inspector에서 체크)

    // 무적 상태 관련
    private bool isInvincible = false; // 무적 여부
    public float invincibleDuration = 1f; // 무적 시간 (1초)

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
        if (amount <= 0) return;   // 음수 피해 무시
        if (isPlayer && isInvincible) return; // 무적 상태면 데미지 무시

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
            else
            {
                // 살아있을 때만 무적 코루틴 실행
                StartCoroutine(Invincibility());
            }

            return; // 플레이어라면 여기서 끝
        }

        // 적 또는 NPC 체력 0 이하 시
        if (!isPlayer && currentHealth <= 0)
        {
            OnDeath?.Invoke(); // 이벤트 호출

            if (isCenterMonster)
            {
                // 부모 그룹이 있다면
                if (transform.parent != null)
                {
                    // 자식 몬스터들 처리
                    foreach (Transform child in transform.parent)
                    {
                        Health childHealth = child.GetComponent<Health>();
                        if (childHealth != null && !childHealth.isCenterMonster)
                        {
                            childHealth.OnDeath?.Invoke();  // 아이템 생성 등 이벤트 호출
                        }
                    }
                    Destroy(transform.parent.gameObject); // 그룹 삭제
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    // 무적 코루틴
    private IEnumerator Invincibility()
    {
        isInvincible = true;

        // Player의 SpriteRenderer 모으기 (자식 포함)
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

        float elapsed = 0f;
        float blinkInterval = 0.2f; // 깜빡이는 간격 (0.2초마다)

        while (elapsed < invincibleDuration)
        {
            // 스프라이트 OFF
            foreach (var sr in srs)
                sr.enabled = false;

            yield return new WaitForSeconds(blinkInterval / 2f);

            // 스프라이트 ON
            foreach (var sr in srs)
                sr.enabled = true;

            yield return new WaitForSeconds(blinkInterval / 2f);

            elapsed += blinkInterval;
        }

        // 무적 종료 시 확실히 보이도록 ON
        foreach (var sr in srs)
            sr.enabled = true;

        isInvincible = false;
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
