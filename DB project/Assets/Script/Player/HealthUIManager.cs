// HealthUIManager.cs
using UnityEngine;
using System.Collections.Generic;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance { get; private set; }

    public GameObject heartPrefab;
    public Transform heartContainer;

    private List<GameObject> heartIcons = new List<GameObject>();
    private bool preventAutoInitialize = false; // Retry 시 초기화 방지

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 씬 전환 시 유지하려면 주석 해제
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 자동 초기화 방지
        if (!preventAutoInitialize)
            InitializeHearts(5); // 기본 체력값
    }

    public void InitializeHearts(int maxHealth)
    {
        if (preventAutoInitialize) return;

        if (heartIcons != null)
        {
            foreach (var h in heartIcons)
            {
                if (h != null) Destroy(h);
            }
            heartIcons.Clear();
        }

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer, false);
            heartIcons.Add(heart);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        if (heartIcons == null) return;

        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (heartIcons[i] == null) continue;
            heartIcons[i].SetActive(i < currentHealth);
        }
    }

    // Retry 시 자동 하트 생성 방지 플래그
    public void SetPreventAutoInitialize(bool value)
    {
        preventAutoInitialize = value;
    }
}
