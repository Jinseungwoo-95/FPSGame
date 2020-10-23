using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyFov))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyFov fov = (EnemyFov)target;

        // 원주 위의 시작점 좌표 계산
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        Handles.color = new Color(1, 1, 1, 0.2f);

        // 외곽선 표현하는 원반 그리기
        Handles.DrawWireDisc(fov.transform.position // 원점 좌표
                            , Vector3.up // 노멀 벡터
                            , fov.viewRange);   // 원의 반지름

        // 부채꼴
        Handles.DrawSolidArc(fov.transform.position
                            , Vector3.up
                            , fromAnglePos  // 부채꼴 시작 좌표
                            , fov.viewAngle // 부채꼴 각도
                            , fov.viewRange);   // 부채꼴 반지름

        // 텍스트
        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f)
                        , fov.viewAngle.ToString());
    }
}
