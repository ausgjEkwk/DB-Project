using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoomUIManager : MonoBehaviour
{
    public GameObject boomIconPrefab;      // 폭탄아이콘 프리팹
    public Transform iconContainer;        // 아이콘들이 놓일 부모 (BoomIcons 오브젝트)
    public int maxBoomCount = 3;

    private List<GameObject> icons = new List<GameObject>();

    public void UpdateBoomUI(int currentBoomCount)
    {
        // 일단 전부 지움
        foreach (GameObject icon in icons)
        {
            Destroy(icon);
        }
        icons.Clear();

        // 현재 개수만큼 생성
        for (int i = 0; i < currentBoomCount; i++)
        {
            GameObject icon = Instantiate(boomIconPrefab, iconContainer);
            icons.Add(icon);
        }
    }
}
