using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooter : MonoBehaviour
{
    [SerializeField] ParticleSystem shootParticle;
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject enemyhitEffect;
    [SerializeField] float fireRate;
    [SerializeField] float range;   // 공격 사거리
    [SerializeField] private LayerMask layerMask;

    float currentFireRate;
    Vector3 originalPos;
    Vector3 recoilPos;
    [SerializeField] Transform camTf;    // 카메라 트랜스폼

    // 불렛 관련
    Image magazineImg;
    [SerializeField] int maxBullet;
    [SerializeField] float reloadTime;
    bool isReloading;
    int curBullet;

    private void Awake()
    {
        originalPos = transform.localPosition;
        recoilPos = originalPos + new Vector3(0, 0, 2f);
        camTf = Camera.main.transform;
        isReloading = false;
        curBullet = maxBullet;
    }

    private void Start()
    {
        magazineImg = GameObject.Find("Magazine").GetComponent<Image>();
    }

    void Update()
    {
        if (PlaySceneManager.instance.PlayState)
        {
            currentFireRate -= Time.deltaTime;

            // 총 재장전
            if(Input.GetKeyDown(KeyCode.R) && curBullet < maxBullet && !isReloading)
            {
                StartCoroutine(ReloadCoroutine());
            }

            // 사격
            if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReloading)
            {
                Shoot();
            }
        }
    }

    void Shoot()
    {
        currentFireRate = fireRate;
        shootParticle.Play();
        SoundManager.Instance.PlayEffect(eTypeEffect.GUN_SHOT);
        Hit();
        StartCoroutine(RecoilCoroutine());
        --curBullet;
        magazineImg.fillAmount = (float)curBullet / maxBullet;

        // 남은 총알이 없을 경우 재장전
        if (curBullet <= 0)
            StartCoroutine(ReloadCoroutine());
    }

    void Hit()
    {
        RaycastHit hit;
        if(Physics.Raycast(camTf.position, camTf.forward, out hit, range, layerMask))
        {
            GameObject hitEffectObj;
            if (hit.transform.CompareTag("Enemy"))
            {
                hitEffectObj = Instantiate(enemyhitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                hitEffectObj.transform.SetParent(hit.transform);
                hit.transform.SendMessage("MinusHP", 1);
            }
            else if (hit.transform.CompareTag("Barrel"))
            {
                hitEffectObj = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                hit.transform.SendMessage("Hit");
            }
            else
            {
                hitEffectObj = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }

            Destroy(hitEffectObj, 1f);
        }
    }

    // 총기 반동
    IEnumerator RecoilCoroutine()
    {
        transform.localPosition = originalPos;

        while(transform.localPosition.z <= 0.15f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, recoilPos, 0.1f);
            yield return null;
        }

        while(transform.localPosition.z >= 0.05f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, 0.2f);
            yield return null;
        }
    }

    // 총 장전
    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        SoundManager.Instance.PlayEffect(eTypeEffect.GUN_RELOAD);
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        magazineImg.fillAmount = 1f;
        curBullet = maxBullet;
    }
}
