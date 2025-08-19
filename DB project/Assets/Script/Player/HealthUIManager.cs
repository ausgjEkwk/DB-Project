using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance { get; private set; }

    public GameObject heartPrefab;
    public string heartContainerName = "HeartContainer"; // �� ������ ã�� Canvas ���� Transform �̸�

    private Transform heartContainer;
    private List<GameObject> heartIcons = new List<GameObject>();
    private bool preventAutoInitialize = false; // Retry �� �� �̵� �� �ʱ�ȭ ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� �̵� �� ����
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� ������ heartContainer�� ã��
        GameObject containerGO = GameObject.Find(heartContainerName);
        if (containerGO != null)
            heartContainer = containerGO.transform;

        // �ڵ� �ʱ�ȭ ���� ���°� �ƴ϶�� ó�� �� ���� ��Ʈ ����
        if (!preventAutoInitialize && heartIcons.Count == 0 && heartContainer != null)
            InitializeHearts(5);
    }

    public void InitializeHearts(int maxHealth)
    {
        if (preventAutoInitialize) return;
        if (heartContainer == null) return;

        foreach (var h in heartIcons)
            if (h != null) Destroy(h);

        heartIcons.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer, false);
            heartIcons.Add(heart);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (heartIcons[i] != null)
                heartIcons[i].SetActive(i < currentHealth);
        }
    }

    public void SetPreventAutoInitialize(bool value)
    {
        preventAutoInitialize = value;
    }
}
