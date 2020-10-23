using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    
    [SerializeField] private List<Transform> wayPoints; // 순찰 지점 저장 List
    [SerializeField] private int nextIdx = 0;   // 다음 순찰 지점 배열 인덱스
    private NavMeshAgent navMeshAgent;

    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;
    private float damping = 1.0f;   // 회전 계수?

    private bool _patrolling;   // 순찰 여부 판단하는 변수

    public bool patrolling
    {
        set
        {
            _patrolling = value;
            if (patrolling)
            {
                navMeshAgent.speed = patrolSpeed;
                damping = 1.0f;
                MoveWayPoint();
            }
        }
        get { return _patrolling; }
    }

    private Vector3 _traceTarget;    // 추적 대상의 위치 저장 변수
    public Vector3 traceTarget
    {
        set
        {
            _traceTarget = value;
            navMeshAgent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
        get { return _traceTarget; }
    }

    public float speed { get { return navMeshAgent.velocity.magnitude; } }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = false;   // 균일한 속도를 유지하기 위해서
        navMeshAgent.updateRotation = false;    // 자동으로 회전하는 기능 끄기
        navMeshAgent.speed = patrolSpeed;

        var group = GameObject.Find("WayPointGroup");

        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);  // 처음에 WayPointGroup의 트랜스폼이 들어가므로 삭제
            nextIdx = Random.Range(0, wayPoints.Count);
        }

        MoveWayPoint();
    }

    private void MoveWayPoint()
    {
        if (navMeshAgent.isPathStale) return;

        navMeshAgent.destination = wayPoints[nextIdx].position;
        navMeshAgent.isStopped = false;
    }

    void Update()
    {
        if(navMeshAgent.isStopped == false)
        {
            // NavMeshAgent가 가야할 방향 벡터를 쿼터니온으로 변환
            Quaternion rot = Quaternion.LookRotation(navMeshAgent.desiredVelocity);

            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);
        }

        if (!_patrolling) return;

        if (navMeshAgent.velocity.sqrMagnitude >= 0.04f
            && navMeshAgent.remainingDistance <= 0.5f)
        {
            nextIdx = Random.Range(0, wayPoints.Count);
            MoveWayPoint();
        }
    }

    void TraceTarget(Vector3 pos)
    {
        if (navMeshAgent.isPathStale) return;

        navMeshAgent.destination = pos;
        navMeshAgent.isStopped = false;
    }

    public void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;   // 바로 정지하기 위해 속도를 0으로 설정
        _patrolling = false;
    }
}
