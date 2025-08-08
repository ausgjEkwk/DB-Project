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
            // �ڽ� �� �̸��� "DArea"�� ������Ʈ ã��, �� �ȿ��� Collider2D ���
            Transform areaTransform = playAreaParent.transform.Find("DArea");
            if (areaTransform != null)
            {
                playAreaBounds = areaTransform.GetComponent<Collider2D>();
            }
        }

        if (playAreaBounds == null)
        {
            Debug.LogError("DArea ������Ʈ�� Collider2D�� ã�� �� �����ϴ�!");
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

