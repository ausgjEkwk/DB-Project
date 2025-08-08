using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    private Collider2D playAreaBounds;

    void Start()
    {
        GameObject playAreaParent = GameObject.Find("PlayArea");

        if (playAreaParent != null)
        {
            // 자식 중 이름이 "DArea"인 오브젝트 찾고, 그 안에서 Collider2D 얻기
            Transform areaTransform = playAreaParent.transform.Find("DArea");
            if (areaTransform != null)
            {
                playAreaBounds = areaTransform.GetComponent<Collider2D>();
            }
        }

        if (playAreaBounds == null)
        {
            Debug.LogError("DArea 오브젝트의 Collider2D를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (playAreaBounds == null) return;

        if (!playAreaBounds.bounds.Contains(transform.position))
        {
            Destroy(gameObject);
        }
    }
}

