using UnityEngine;

public class BulletBoundaryChecker : MonoBehaviour
{
    private BoxCollider2D areaCollider;

    void Start()
    {
        // DArea BoxCollider2D ��������
        GameObject dArea = GameObject.Find("DArea");
        if (dArea != null)
        {
            areaCollider = dArea.GetComponent<BoxCollider2D>();
        }
        else
        {
            Debug.LogError("DArea�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        if (areaCollider == null) return;

        // PBullet �±� ���� ��� ������Ʈ ��������
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
