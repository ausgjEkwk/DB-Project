using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance { get; private set; }

    public GameObject heartPrefab;
    public string heartContainerName = "HeartContainer"; // 새 씬에서 찾을 Canvas 하위 Transform 이름

    private Transform heartContainer;
    private List<GameObject> heartIcons = new List<GameObject>();
    private bool preventAutoInitialize = false; // Retry 및 씬 이동 시 초기화 방지

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 시 유지
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
        // 새 씬에서 heartContainer를 찾음
        GameObject containerGO = GameObject.Find(heartContainerName);
        if (containerGO != null)
            heartContainer = containerGO.transform;

        // 자동 초기화 방지 상태가 아니라면 처음 한 번만 하트 생성
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
