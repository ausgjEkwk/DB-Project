using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;       // 최대 체력
    public int currentHealth;       // 현재 체력
    public bool isPlayer;           // 플레이어 여부

    // 체력 0이 되어 죽었을 때 호출되는 이벤트 (외부 구독 가능)
    public event Action OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            // 플레이어 처리
            if (isPlayer)
            {
                Debug.Log("플레이어 사망");
                OnDeath?.Invoke(); // 이벤트 호출
                // TODO: 게임오버 처리
            }
            else
            {
                // 몬스터 처리
                OnDeath?.Invoke(); // 이벤트 호출

                if (IsCenterMonster())
                {
                    // 가운데 몬스터면 부모 그룹 전체 삭제
                    if (transform.parent != null)
                        Destroy(transform.parent.gameObject);
                    else
                        Destroy(gameObject);
                }
                else
                {
                    // 양 옆 몬스터면 자기 자신만 삭제
                    Destroy(gameObject);
                }
            }
        }
    }

    public void Heal(int amount)
    {
        if (!isPlayer) return;
        if (amount <= 0) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    // 가운데 몬스터 판단: 이름 끝에 "(숫자)"가 없는 경우 true
    private bool IsCenterMonster()
    {
        string objName = gameObject.name;

        // "(숫자)"로 끝나면 가운데 몬스터 아님
        if (objName.EndsWith(")"))
            return false;

        // MonsterA, MonsterB, MonsterC 라면 가운데 몬스터
        if (objName.StartsWith("MonsterA") || objName.StartsWith("MonsterB") || objName.StartsWith("MonsterC"))
            return true;

        return false;
    }
}
