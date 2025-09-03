using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRemover : MonoBehaviour
{
    // Collider2D가 이 오브젝트 영역 밖으로 나갈 때 호출되는 함수
    private void OnTriggerExit2D(Collider2D other)
    {
        // 나간 오브젝트가 "Bullet" 태그를 가진 경우
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // 총알 제거
        }
    }
}
