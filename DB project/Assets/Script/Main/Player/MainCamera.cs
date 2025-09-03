using UnityEngine;

[RequireComponent(typeof(Camera))] // 반드시 Camera 컴포넌트가 붙어야 함
public class CameraReset : MonoBehaviour
{
    private Camera cam; // 카메라 컴포넌트 참조

    private void Start()
    {
        cam = GetComponent<Camera>(); // 현재 오브젝트의 Camera 컴포넌트 가져오기

        if (!cam.orthographic) // 카메라가 Orthographic이 아니면
        {
            Debug.LogError("이 스크립트는 Orthographic 카메라에서만 작동합니다."); // 경고 출력
            return; // 실행 중단
        }

        // 카메라 비율과 Viewport Rect를 기본값으로 설정
        cam.aspect = (float)Screen.width / Screen.height; // 화면 비율에 맞춰 Aspect 조정
        cam.rect = new Rect(0f, 0f, 1f, 1f);            // 화면 전체 사용 (x, y, width, height)
    }
}
