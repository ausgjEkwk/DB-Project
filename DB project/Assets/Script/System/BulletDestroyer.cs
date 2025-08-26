using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    private Collider2D playAreaBounds; // �Ѿ��� ������ �� �ִ� ������ Collider2D�� ����

    void Start()
    {
        // "PlayArea"��� �̸��� �θ� ������Ʈ ã��
        GameObject playAreaParent = GameObject.Find("PlayArea");

        if (playAreaParent != null)
        {
            // PlayArea�� �ڽ� �� �̸��� "DArea"�� ������Ʈ ã��
            Transform areaTransform = playAreaParent.transform.Find("DArea");

            if (areaTransform != null)
            {
                // "DArea" ������Ʈ���� Collider2D ������Ʈ ��������
                playAreaBounds = areaTransform.GetComponent<Collider2D>();
            }
        }

        // ���� Collider2D�� ã�� �������� ��� �޽��� ���
        if (playAreaBounds == null)
        {
            Debug.LogError("DArea ������Ʈ�� Collider2D�� ã�� �� �����ϴ�!");
        }
    }

    void Update()
    {
        // Collider2D�� ������ �ƹ��͵� ���� ����
        if (playAreaBounds == null) return;

        // �Ѿ� ��ġ�� �÷��� ���� �ٱ��̸� �Ѿ� ����
        if (!playAreaBounds.bounds.Contains(transform.position))
        {
            Destroy(gameObject); // �Ѿ� ����
        }
    }
}
