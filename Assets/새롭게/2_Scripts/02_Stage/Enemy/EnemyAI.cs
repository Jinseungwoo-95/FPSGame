using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;
    private Transform playerTr;

    [SerializeField] private float attackDist = 10.0f;
    public bool isDie = false;
    private WaitForSeconds waitSecond;

    private MoveAgent moveAgent;
    private EnemyFire enemyFire;

    // Animator 
    private Animator animator;
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");

    //HP
    [SerializeField] int maxHP = 10;
    int curHP;

    // HP Bar
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] Vector3 hpBarOffset;
    private GameObject hpBar;
    private Canvas EnemyHPCanvas;
    private Image hpBarImg;

    // 시야각
    private EnemyFov enemyFov;

    private void Awake()
    {
        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();
        enemyFov = GetComponent<EnemyFov>();

        waitSecond = new WaitForSeconds(0.3f);
        curHP = maxHP;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTr = player.GetComponent<Transform>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
        //PlaySceneManager.instance.Endevent += DestroyObjEvent;
    }

    private void OnDisable()
    {
        //PlaySceneManager.instance.Endevent -= DestroyObjEvent;
    }

    private void Start()
    {
        SetHpBar();
    }

    private void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    IEnumerator CheckState()
    {
        //yield return new WaitForSeconds(1.0f);

        while(!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playerTr.position, transform.position);

            if(dist <= attackDist)
            {
                if (enemyFov.isViewPlayer())
                    state = State.ATTACK;
                else
                    state = State.TRACE;
            }
            else if(enemyFov.isTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return waitSecond;
        }
    }

    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return waitSecond;

            switch(state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;

                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    if(enemyFire.isFire == false) enemyFire.isFire = true;
                    break;

                case State.DIE:
                    isDie = true;
                    enemyFire.isFire = false;
                    moveAgent.Stop();
                    Destroy(hpBar);
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    void MinusHP(int _damage)
    {
        Debug.Log("호출");
        curHP -= _damage;
        hpBarImg.fillAmount = (float)curHP / maxHP;

        if (curHP <= 0)
        {
            state = State.DIE;
        }
    }
    void SetHpBar()
    {
        hpBarOffset = new Vector3(0, GetComponent<CapsuleCollider>().height, 0);

        EnemyHPCanvas = GameObject.Find("EnemyHPCanvas").GetComponent<Canvas>();
        hpBar = Instantiate(hpBarPrefab, EnemyHPCanvas.transform);
        hpBarImg = hpBar.transform.GetChild(0).GetComponent<Image>();

        EnemyHPBar enemyHPBar = hpBar.GetComponent<EnemyHPBar>();
        enemyHPBar.enemyTf = transform;
        enemyHPBar.offset = hpBarOffset;
    }

    void DestroyObjEvent()
    {
        Destroy(hpBar);
        Destroy(gameObject);
    }
}
