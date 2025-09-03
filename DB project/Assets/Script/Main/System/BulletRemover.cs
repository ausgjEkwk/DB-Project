using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRemover : MonoBehaviour
{
    // Collider2D�� �� ������Ʈ ���� ������ ���� �� ȣ��Ǵ� �Լ�
    private void OnTriggerExit2D(Collider2D other)
    {
        // ���� ������Ʈ�� "Bullet" �±׸� ���� ���
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // �Ѿ� ����
        }
    }
}
