using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUtility : MonoBehaviour
{
    // 카메라 컴포넌트에 대한 참조 변수
    public Camera _camera;

    // 픽셀당 월드 단위 크기에 대한 변수
    public float pixelsToWorldUnits;

    public void UpdateCameraSize()
    {
        // 직교 좌표계의 크기를 업데이트한다
        _camera.orthographicSize = (Screen.height * 0.5f) / pixelsToWorldUnits;
    }

    // 렌더러가 특정 카메라의 프러스텀에 포함되었는지를 감지하는 함수
    // 렌더러가 프러스텀 내에 있으면 true, 아니면 false를 반환한다
    public static bool IsRendererInFrustum(Renderer renderable, Camera camera)
    {
        // 카메라에서 프러스텀 평면을 생성한다
        // 각 평면은 프러스텀의 벽 한 면을 나타내는 것이다
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        // 프러스텀 평면 안에 렌더링 가능한 것이 있는지 검사한다
        return GeometryUtility.TestPlanesAABB(planes, renderable.bounds);
    }

    // 씬 안의 점이 지정된 카메라의 프러스텀 안에 위치하는지를 검사하는 함수
    // 점이 프러스텀 내에 있으면 true, 아니면 false를 반환한다
    // 출력 파라미터인 ViewPortLoc는 함수가 true를 반환할 때 화면상의 점 위치를 알려준다
    public static bool IsPointInFrustum(Vector3 point, Camera camera, out Vector3 viewportLocation)
    {
        // 크기가 없는 경계를 생성한다
        Bounds bounds = new Bounds(point, Vector3.zero);

        // 카메라에서 프러스텀 평면을 생성한다
        // 각 평면은 프러스텀의 벽 한 면을 나타내는 것이다
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        // 프러스텀 평면 안에 점이 있는지 검사한다
        bool bVisible = GeometryUtility.TestPlanesAABB(planes, bounds);

        // 뷰포트 위치 변수에 값을 할당한다
        viewportLocation = Vector3.zero;

        // 점이 보이는 상태이면 점의 뷰포트상 위치를 구한다
        if (bVisible == true)
        {
            viewportLocation = camera.WorldToViewportPoint(point);
        }

        return bVisible;
    }
}
