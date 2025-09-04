using UnityEngine;

public class BulletBoundaryChecker : MonoBehaviour
{
    private BoxCollider2D areaCollider;

    void Start()
    {
        // DArea BoxCollider2D 가져오기
        GameObject dArea = GameObject.Find("DArea");
        if (dArea != null)
        {
            areaCollider = dArea.GetComponent<BoxCollider2D>();
        }
        else
        {
            Debug.LogError("DArea를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (areaCollider == null) return;

        // PBullet 태그 가진 모든 오브젝트 가져오기
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("PBullet");
        foreach (GameObject bullet in bullets)
        {
            if (!areaCollider.bounds.Contains(bullet.transform.position))
            {
                Destroy(bullet);
            }
        }
    }
}
