using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public Renderer backgroundRenderer;  // MeshRenderer 컴포넌트 할당
    public float scrollSpeed = 0.5f;     // 스크롤 속도 조절 변수

    private void Update()
    {
        float offset = Time.time * scrollSpeed;
        backgroundRenderer.material.mainTextureOffset = new Vector2(0, offset);
    }
}