using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static HealthUIManager Instance { get; private set; }

    // ��Ʈ ������ ������ (�ν����Ϳ� �Ҵ�)
    public GameObject heartPrefab;

    // ��Ʈ �����ܵ��� ������ �θ� ������Ʈ (�ν����Ϳ� �Ҵ�)
    public Transform heartContainer;

    // ��Ʈ ������ �迭
    private GameObject[] heartIcons;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �� ��ȯ�� �����Ϸ��� �ּ� ����
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �ִ� ü�¿� ���� ��Ʈ ������ �ʱ�ȭ
    public void InitializeHearts(int maxHealth)
    {
        // ���� ��Ʈ ������ ��� ����
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        heartIcons = new GameObject[maxHealth];

        // maxHealth ��ŭ ��Ʈ ������ ���� �� ����
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            heartIcons[i] = heart;
        }
    }

    // ���� ü�¿� �°� ��Ʈ ������ Ȱ��/��Ȱ�� ����
    public void UpdateHearts(int currentHealth)
    {
        if (heartIcons == null) return;

        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < currentHealth);
        }
    }
}
