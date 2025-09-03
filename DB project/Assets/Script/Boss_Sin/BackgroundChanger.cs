using UnityEngine;

public class BackgroundChanger : MonoBehaviour
{
    [Header("BackGround MeshRenderer")]
    public MeshRenderer bgRenderer; // BackGround 오브젝트의 MeshRenderer

    [Header("보스 씬용 배경")]
    public Texture bossTexture;     // 바꿀 텍스처

    // 배경을 보스용 이미지로 변경하는 함수
    public void ChangeToBossBackground()
    {
        if (bgRenderer != null && bossTexture != null)
        {
            bgRenderer.material.mainTexture = bossTexture;
            Debug.Log("배경이 보스용으로 변경되었습니다!");
        }
        else
        {
            Debug.LogWarning("BackGround 또는 보스 텍스처가 할당되지 않았습니다.");
        }
    }
}
