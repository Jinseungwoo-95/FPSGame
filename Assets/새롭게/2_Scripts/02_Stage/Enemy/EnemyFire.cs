using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private AudioSource audio;
    private Animator animator;
    private Transform playerTr;

    private readonly int hashFire = Animator.StringToHash("Fire");
    private readonly int hashReload = Animator.StringToHash("Reload");

    // Fire
    private float nextFire = 0.0f;
    private readonly float fireRate = 1f;
    private readonly float damping = 10.0f;

    public bool isFire = false;
    [SerializeField] private AudioClip fireSfx;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePos;

    // Reload
    private readonly float reloadTime = 2.0f;
    private readonly int maxBullet = 10;
    private int curBullet = 10;
    private bool isReload = false;
    [SerializeField] AudioClip reloadSfx;
    private WaitForSeconds wsReload;

    // MuzzleFlash
    [SerializeField] private MeshRenderer muzzleFlash;
    private WaitForSeconds wsMuzzle;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        wsReload = new WaitForSeconds(reloadTime);
        wsMuzzle = new WaitForSeconds(0.2f);
    }

    void Update()
    {
        if(!isReload && isFire)
        {
            Vector3 dir = playerTr.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);

            if (Vector3.Angle(transform.forward, dir) <15.0f && Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate;
            }
        }
    }

    private void Fire()
    {
        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1.0f);
        StartCoroutine(ShowMuzzleFlash());

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        Destroy(bullet, 3.0f);

        isReload = (--curBullet == 0);

        if(isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        muzzleFlash.enabled = false;
        animator.SetTrigger(hashReload);
        audio.PlayOneShot(reloadSfx, 1.0f);

        yield return wsReload;

        curBullet = maxBullet;
        isReload = false;
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true;

        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        // "_MainTex"
        // 미리 지정되어 있는 프로퍼티 Name으로 Diffuse를 나타냄
        // 메테리얼의 offset 변경
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);  

        yield return wsMuzzle;
        muzzleFlash.enabled = false;
    }
}
