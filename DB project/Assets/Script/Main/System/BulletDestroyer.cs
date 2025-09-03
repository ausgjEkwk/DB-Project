using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    private Collider2D playAreaBounds; // 총알이 존재할 수 있는 영역의 Collider2D를 저장

    void Start()
    {
        // "PlayArea"라는 이름의 부모 오브젝트 찾기
        GameObject playAreaParent = GameObject.Find("PlayArea");

        if (playAreaParent != null)
        {
            // PlayArea의 자식 중 이름이 "DArea"인 오브젝트 찾기
            Transform areaTransform = playAreaParent.transform.Find("DArea");

            if (areaTransform != null)
            {
                // "DArea" 오브젝트에서 Collider2D 컴포넌트 가져오기
                playAreaBounds = areaTransform.GetComponent<Collider2D>();
            }
        }

        // 만약 Collider2D를 찾지 못했으면 경고 메시지 출력
        if (playAreaBounds == null)
        {
            Debug.LogError("DArea 오브젝트의 Collider2D를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        // Collider2D가 없으면 아무것도 하지 않음
        if (playAreaBounds == null) return;

        // 총알 위치가 플레이 영역 바깥이면 총알 삭제
        if (!playAreaBounds.bounds.Contains(transform.position))
        {
            Destroy(gameObject); // 총알 제거
        }
    }
}
