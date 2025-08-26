using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager Instance { get; private set; } // 싱글톤 접근용

    public GameObject heartPrefab;                  // 하트 아이콘 프리팹
    public string heartContainerName = "HeartContainer"; // Canvas 하위에 하트 배치할 Transform 이름

    private Transform heartContainer;              // 하트 아이콘을 배치할 부모 Transform
    private List<GameObject> heartIcons = new List<GameObject>(); // 생성된 하트 아이콘 리스트
    private bool preventAutoInitialize = false;    // Retry/씬 전환 시 자동 초기화 방지

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;                       // 싱글톤 등록
            DontDestroyOnLoad(gameObject);         // 씬 전환 시 오브젝트 유지
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 후 처리 콜백
        }
        else
        {
            Destroy(gameObject);                   // 이미 존재하면 중복 제거
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새 씬에서 heartContainer를 찾음
        GameObject containerGO = GameObject.Find(heartContainerName);
        if (containerGO != null)
            heartContainer = containerGO.transform;

        // 자동 초기화 방지 상태가 아니고, 하트가 없으면 기본 5개 생성
        if (!preventAutoInitialize && heartIcons.Count == 0 && heartContainer != null)
            InitializeHearts(5);
    }

    public void InitializeHearts(int maxHealth)
    {
        if (preventAutoInitialize) return;         // 초기화 방지 중이면 무시
        if (heartContainer == null) return;        // 부모 Transform 없으면 무시

        foreach (var h in heartIcons)
            if (h != null) Destroy(h);             // 기존 하트 삭제

        heartIcons.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer, false); // 하트 생성
            heartIcons.Add(heart);                  // 리스트에 추가
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        // 현재 체력만큼 하트 활성화, 나머지는 비활성화
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (heartIcons[i] != null)
                heartIcons[i].SetActive(i < currentHealth);
        }
    }

    public void SetPreventAutoInitialize(bool value)
    {
        preventAutoInitialize = value;             // 외부에서 초기화 방지 여부 설정
    }
}
