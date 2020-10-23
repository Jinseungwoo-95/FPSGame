using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum eCameraActionState
    {
        MOVEWARK = 0,
        PLAYERSEEKER,
    }

    [SerializeField] float lookSensitivity = 2f;
    [SerializeField] float cameraRotationLimit = 45f;

    eCameraActionState eCamState;

    // MOVEWARK 관련 변수
    [SerializeField] float moveSpeed;
    Transform movePointRoot;
    List<Transform> points;
    Vector3 startPos;
    int currentIndex;

    // PLAYERSEEKER 관련 변수
    float currentCameraRotationX = 0f;

    private void Awake()
    {
        eCamState = eCameraActionState.MOVEWARK;
        points = new List<Transform>();
        currentIndex = 0;
        startPos = transform.position;

#if UNITY_EDITOR
        moveSpeed = 70f;
#else
        moveSpeed = 30f;
#endif

    }

    void Start()
    {
        movePointRoot = GameObject.Find("CameraRoot").transform;

        for (int i = 0; i < movePointRoot.childCount; i++)
        {
            points.Add(movePointRoot.GetChild(i));
        }

        transform.position = points[currentIndex++].position;
        transform.LookAt(movePointRoot);
    }

    void Update()
    {
        if (eCamState == eCameraActionState.MOVEWARK)
        {
            if (currentIndex < movePointRoot.childCount)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[currentIndex].position, Time.deltaTime * moveSpeed);
                transform.LookAt(movePointRoot);

                if (Vector3.Distance(transform.position, points[currentIndex].position) <= 0.1f)
                    currentIndex++;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * moveSpeed);
                transform.LookAt(movePointRoot);

                // 플레이어 위치까지 이동
                if (Vector3.Distance(transform.position, startPos) <= 0.1f)
                {
                    transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
                    transform.GetChild(0).gameObject.SetActive(true);
                    transform.localPosition = new Vector3(0, 1f, 0);
                    transform.localEulerAngles = Vector3.zero;
                    eCamState = eCameraActionState.PLAYERSEEKER;
                    PlaySceneManager.instance.ChangeState(eGameState.START);
                }
            }
        }
        else
        {
            if (PlaySceneManager.instance.PlayState)
                Rotate();
        }
    }

    void Rotate()
    {
        // 마우스 y축 움직임에 대한 X축 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
        transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }
}
