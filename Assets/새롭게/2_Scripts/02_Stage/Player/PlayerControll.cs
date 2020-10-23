using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float lookSensitivity = 2f;
    [SerializeField] float maxHP;

    float curHP;
    bool isDeath;
    float moveSpeed;
    bool isGround;

    Rigidbody myRigid;
    BoxCollider collider;
    Image bloodScreen;
    Image hpBar;

    Vector3 velocity;

    private void Awake()
    {
        //maxHP = 1;  // 나중에 삭제하기
        moveSpeed = walkSpeed;
        curHP = maxHP;
        myRigid = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();

    }

    private void Start()
    {
        bloodScreen = GameObject.Find("BloodScreen").GetComponent<Image>();
        hpBar = GameObject.Find("HPBar").GetComponent<Image>();
    }

    void Update()
    {
        if (isDeath)
            return;

        if (PlaySceneManager.instance.PlayState)
        {
            //Jump();
            Move();
            Rotate();
        }
    }

    private void FixedUpdate()
    {
        if (PlaySceneManager.instance.PlayState)
            myRigid.MovePosition(transform.position + velocity * Time.deltaTime * moveSpeed);
    }

    //void Jump()
    //{
    //    isGround = Physics.Raycast(transform.position, Vector3.down, collider.bounds.extents.y + 0.2f);
    //    if (Input.GetKeyDown(KeyCode.Space) && isGround)
    //    {
    //        myRigid.velocity = Vector3.up * jumpForce;
    //    }
    //}

    void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        velocity = (_moveHorizontal + _moveVertical).normalized;
    }

    void Rotate()
    {
        // 마우스 x축 움직임에 대한 Y축 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotatinY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotatinY));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MinusHP();
        }
    }

    void MinusHP(int _damage = 1)
    {
        StartCoroutine(ShowBloodScreen());
        curHP -= _damage;
        hpBar.fillAmount = curHP / maxHP;
        if (curHP <= 0)
        {
            isDeath = true;
            //myRigid.useGravity = false;
            //collider.enabled = false;
            PlaySceneManager.instance.ChangeState(eGameState.END);
        }
    }

    IEnumerator ShowBloodScreen()
    {
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.3f, 0.5f));
        yield return new WaitForSeconds(0.3f);
        bloodScreen.color = Color.clear;  
    }

    public void ResetInfo()
    {
        curHP = maxHP;
        transform.position = Vector3.zero;
    }
}
