using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static HealthUIManager Instance { get; private set; }

    // 하트 아이콘 프리팹 (인스펙터에 할당)
    public GameObject heartPrefab;

    // 하트 아이콘들이 생성될 부모 오브젝트 (인스펙터에 할당)
    public Transform heartContainer;

    // 하트 아이콘 배열
    private GameObject[] heartIcons;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬 전환시 유지하려면 주석 해제
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 최대 체력에 맞춰 하트 아이콘 초기화
    public void InitializeHearts(int maxHealth)
    {
        // 기존 하트 아이콘 모두 삭제
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        heartIcons = new GameObject[maxHealth];

        // maxHealth 만큼 하트 아이콘 생성 및 저장
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            heartIcons[i] = heart;
        }
    }

    // 현재 체력에 맞게 하트 아이콘 활성/비활성 조절
    public void UpdateHearts(int currentHealth)
    {
        if (heartIcons == null) return;

        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < currentHealth);
        }
    }
}
