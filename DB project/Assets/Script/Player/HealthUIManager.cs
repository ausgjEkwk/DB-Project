// HealthUIManager.cs
using UnityEngine;
using System.Collections.Generic;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance { get; private set; }

    public GameObject heartPrefab;
    public Transform heartContainer;

    private List<GameObject> heartIcons = new List<GameObject>();
    private bool preventAutoInitialize = false; // Retry �� �ʱ�ȭ ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // �� ��ȯ �� �����Ϸ��� �ּ� ����
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // �ڵ� �ʱ�ȭ ����
        if (!preventAutoInitialize)
            InitializeHearts(5); // �⺻ ü�°�
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

    // Retry �� �ڵ� ��Ʈ ���� ���� �÷���
    public void SetPreventAutoInitialize(bool value)
    {
        preventAutoInitialize = value;
    }
}
