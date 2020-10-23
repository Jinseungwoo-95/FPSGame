using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class ZombieController : MonoBehaviour
{
    public enum eActionState
    {
        IDLE = 0,
        RUN,
        ATTACK,
        DEATH,
        WALK,
    }

    [SerializeField] float walkSpeed = 0.5f;
    [SerializeField] float runSpeed = 3f;
    [SerializeField] float limitX = 5f;
    [SerializeField] float limitZ = 5f;
    [SerializeField] float minWaitTime = 2f;
    [SerializeField] float maxWaitTime = 5f;
    [SerializeField] BoxCollider damageZone;
    [SerializeField] int maxHP = 10;

    // HP 관련
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] Vector3 hpBarOffset;
    private GameObject hpBar;
    private Canvas EnemyHPCanvas;
    private Image hpBarImg;


    BoxCollider collider;
    int curHP;
    eActionState stateAction;
    Animator animator;
    bool isDeath;
    bool isAttack;
    NavMeshAgent navAgent;
    Vector3 startPos;
    float waitTime;
    bool isSelectAI;
    Transform player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        collider = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
        isDeath = false;
        isAttack = false;
        waitTime = 0;
        curHP = maxHP;
        hpBarOffset = new Vector3(0, collider.size.y, 0);
    }

    private void Start()
    {
        SetHpBar();
    }

    private void OnEnable()
    {
        PlaySceneManager.instance.Endevent += DestroyObjEvent;
    }

    private void OnDisable()
    {
        PlaySceneManager.instance.Endevent -= DestroyObjEvent;
    }

    private void Update()
    {
        if (isDeath)
            return;

        if (PlaySceneManager.instance.PlayState)
        {
            switch (stateAction)
            {
                case eActionState.IDLE:
                    waitTime -= Time.deltaTime;
                    if (Vector3.Distance(transform.position, player.position) <= 20f)
                    {
                        ChangedAction(eActionState.RUN);
                    }
                    else if (waitTime <= 0)
                    {
                        // 다시 선택
                        ProcessAI();
                    }
                    break;

                case eActionState.WALK:
                    if (Vector3.Distance(transform.position, player.position) <= 20f)
                    {
                        ChangedAction(eActionState.RUN);
                    }
                    else if (navAgent.remainingDistance <= 0.1f)
                    {
                        // 다시 선택
                        ProcessAI();
                    }
                    break;

                case eActionState.RUN:
                    if (navAgent.remainingDistance <= 2.15f)
                    {
                        ChangedAction(eActionState.ATTACK);
                    }
                    else
                    {
                        navAgent.destination = player.position;
                    }
                    break;

                case eActionState.ATTACK:
                    Vector3 rot = Quaternion.LookRotation(player.position - transform.position).eulerAngles;
                    transform.eulerAngles = new Vector3(0, rot.y, 0);

                    if (!isAttack && Vector3.Distance(transform.position, player.position) > 2.1f)
                    {
                        ChangedAction(eActionState.RUN);
                    }
                    break;
            }
        }
    }

    void ProcessAI()
    {
        if (!isSelectAI)
        {
            int r = Random.Range(0, 2);

            if (r == 0)
            {
                // 대기
                ChangedAction(eActionState.IDLE);
                waitTime = Random.Range(minWaitTime, maxWaitTime);

            }
            else
            {
                // 이동
                ChangedAction(eActionState.WALK);
                navAgent.destination = GetRandomPos(startPos, limitX, limitZ);
            }
        }
    }

    Vector3 GetRandomPos(Vector3 center, float limitX, float limitZ)
    {
        float rX = Random.Range(-limitX, limitX);
        float rZ = Random.Range(-limitZ, limitZ);

        Vector3 rv = new Vector3(rX, 0, rZ);
        return center + rv;
    }

    void ChangedAction(eActionState state)
    {
        switch(state)
        {
            case eActionState.IDLE:
                ChangeState(state);
                break;
            case eActionState.WALK:
                if(stateAction != eActionState.ATTACK)
                {
                    navAgent.stoppingDistance = 0f;
                    navAgent.speed = walkSpeed;
                    ChangeState(state);
                }
                break;
            case eActionState.RUN:
                navAgent.destination = player.transform.position;
                navAgent.stoppingDistance = 2f;
                navAgent.speed = runSpeed;
                ChangeState(state);
                break;
            case eActionState.ATTACK:
                if(stateAction == eActionState.RUN)
                {
                    isAttack = true;
                    ChangeState(state);
                }
                break;
            case eActionState.DEATH:
                PlaySceneManager.instance.MinusEnemyCnt();
                Destroy(hpBar);
                SoundManager.Instance.PlayEffect(eTypeEffect.ZOMBIE_DIE);
                isDeath = true;
                navAgent.velocity = Vector3.zero;
                navAgent.isStopped = true;
                collider.enabled = false;
                ChangeState(state);
                break;
        }
    }


    /// <summary>
    /// 현재 스테이트와 애니메이션 변경
    /// </summary>
    /// <param name="state"></param>
    void ChangeState(eActionState state)
    {
        stateAction = state;
        animator.SetInteger("AniState", (int)stateAction);
    }

    /// <summary>
    ///  애니메이션 이벤트로 데미지존 박스콜라이더 온
    /// </summary>
    void DamageZoneOn()
    {
        isAttack = true;
        damageZone.enabled = true;
    }

    /// <summary>
    /// 애니메이션 이벤트로 데미지존 박스콜라이더 오프
    /// </summary>
    void DamageZoneOff()
    {
        isAttack = false;
        damageZone.enabled = false;
    }

    /// <summary>
    /// Shooter 클래스에서 메시지로 호출됨
    /// </summary>
    void MinusHP(int _damage)
    {
        if (stateAction == eActionState.IDLE || stateAction == eActionState.WALK)
            ChangedAction(eActionState.RUN);

        curHP -= _damage;
        hpBarImg.fillAmount = (float)curHP / maxHP;

        if(curHP <= 0)
        {
            ChangedAction(eActionState.DEATH);
        }
    }

    // 애니메이션 이벤트로 호출 됨.
    void DestroyObj()
    {
        Destroy(gameObject);
    }

    // PlaySceneManager에서 엔드이벤트로 호출 됨
    void DestroyObjEvent()
    {
        Destroy(hpBar);
        Destroy(gameObject);
    }

    void SetHpBar()
    {
        EnemyHPCanvas = GameObject.Find("EnemyHPCanvas").GetComponent<Canvas>();
        hpBar = Instantiate(hpBarPrefab, EnemyHPCanvas.transform);
        hpBarImg = hpBar.transform.GetChild(0).GetComponent<Image>();

        EnemyHPBar enemyHPBar = hpBar.GetComponent<EnemyHPBar>();
        enemyHPBar.enemyTf = transform;
        enemyHPBar.offset = hpBarOffset;
    }
}
