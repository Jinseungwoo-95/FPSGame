using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    private Camera EnemyCamera;
    private Canvas EnemyCanvas;
    private RectTransform rectParent;
    private RectTransform rectHP;

    [HideInInspector]
    public Vector3 offset = Vector3.zero;
    [HideInInspector]
    public Transform enemyTf;

    private Camera mainCamera;

    void Start()
    {
        EnemyCanvas = GetComponentInParent<Canvas>();
        EnemyCamera = EnemyCanvas.worldCamera;
        rectParent = EnemyCanvas.GetComponent<RectTransform>();
        rectHP = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(enemyTf.position + offset);

        // 뒤로 돌았을 경우에도 hpbar가 보이는것을 막기 위해서.
        if(screenPos.z < 0f)
        {
            screenPos *= -1f;
        }

        Vector2 localPos = Vector2.zero;
        // 스크린 포인트에서 UI 좌표로 변경해줌
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, EnemyCamera, out localPos);
        //rectHP.localPosition = localPos;
        rectHP.anchoredPosition = localPos;
    }
}

