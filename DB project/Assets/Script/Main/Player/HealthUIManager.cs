using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance { get; private set; } // �̱��� ���ٿ�

    public GameObject heartPrefab;                  // ��Ʈ ������ ������
    public string heartContainerName = "HeartContainer"; // Canvas ������ ��Ʈ ��ġ�� Transform �̸�

    private Transform heartContainer;              // ��Ʈ �������� ��ġ�� �θ� Transform
    private List<GameObject> heartIcons = new List<GameObject>(); // ������ ��Ʈ ������ ����Ʈ
    private bool preventAutoInitialize = false;    // Retry/�� ��ȯ �� �ڵ� �ʱ�ȭ ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                       // �̱��� ���
            DontDestroyOnLoad(gameObject);         // �� ��ȯ �� ������Ʈ ����
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �� ó�� �ݹ�
        }
        else
        {
            Destroy(gameObject);                   // �̹� �����ϸ� �ߺ� ����
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� ������ heartContainer�� ã��
        GameObject containerGO = GameObject.Find(heartContainerName);
        if (containerGO != null)
            heartContainer = containerGO.transform;

        // �ڵ� �ʱ�ȭ ���� ���°� �ƴϰ�, ��Ʈ�� ������ �⺻ 5�� ����
        if (!preventAutoInitialize && heartIcons.Count == 0 && heartContainer != null)
            InitializeHearts(5);
    }

    public void InitializeHearts(int maxHealth)
    {
        if (preventAutoInitialize) return;         // �ʱ�ȭ ���� ���̸� ����
        if (heartContainer == null) return;        // �θ� Transform ������ ����

        foreach (var h in heartIcons)
            if (h != null) Destroy(h);             // ���� ��Ʈ ����

        heartIcons.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer, false); // ��Ʈ ����
            heartIcons.Add(heart);                  // ����Ʈ�� �߰�
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        // ���� ü�¸�ŭ ��Ʈ Ȱ��ȭ, �������� ��Ȱ��ȭ
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (heartIcons[i] != null)
                heartIcons[i].SetActive(i < currentHealth);
        }
    }

    public void SetPreventAutoInitialize(bool value)
    {
        preventAutoInitialize = value;             // �ܺο��� �ʱ�ȭ ���� ���� ����
    }
}
