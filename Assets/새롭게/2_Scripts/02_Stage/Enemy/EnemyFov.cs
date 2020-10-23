using UnityEngine;

public class EnemyFov : MonoBehaviour
{
    public float viewRange = 15.0f; // 적 추적 사정 거리 범위
    public float viewAngle = 120.0f; // 적 시야각

    private Transform playerTr;
    private int playerLayer;
    [SerializeField] LayerMask layerMask;

    private void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        playerLayer = 1 << LayerMask.NameToLayer("Player");
    }

    // 주어진 각도에 의해 원주 위의 점 좌표값을 계산하는 함수
    public Vector3 CirclePoint(float angle)
    {
        angle += transform.eulerAngles.y;
        
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool isTracePlayer()
    {
        bool isTrace = false;

        // 추적 반경 범위 안에서 주인공 캐릭터 추출
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewRange, playerLayer);

        if (colliders.Length == 1)
        {
            // 플리어와의 방향 벡터 계산
            Vector3 dir = (playerTr.position - transform.position).normalized;

            // 시야각에 플레이어가 들어와있는지 판단
            if (Vector3.Angle(transform.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }

        return isTrace;
    }

    public bool isViewPlayer()
    {
        bool isView = false;

        RaycastHit hit;

        Vector3 dir = (playerTr.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, dir, out hit, viewRange, layerMask))
        {
            isView = hit.collider.CompareTag("Player");
        }

        return isView;
    }
}
