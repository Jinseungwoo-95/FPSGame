using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarrelScript : MonoBehaviour
{
    [SerializeField] private GameObject expEffect;
    [SerializeField] private float expFoce;
    [SerializeField] LayerMask expLayer;
    [SerializeField] private float expRadius;
    private MeshCollider collider;
    private MeshRenderer renderer;
    private int hitCount;
    private Rigidbody rb;
    private Vector3 originPos;
    private Quaternion originRot;

    void Awake()
    {
        originPos = transform.position;
        originRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<MeshCollider>();
        renderer = GetComponent<MeshRenderer>();
        hitCount = 0;
    }

    void Hit()
    {
        if(++hitCount == 3)
        {
            ExpBarrel();
        }
    }

    /// <summary>
    /// 폭발 메서드
    /// </summary>
    void ExpBarrel()
    {
        SoundManager.Instance.PlayEffect(eTypeEffect.BARREL_EXP);
        Instantiate(expEffect, transform.position, Quaternion.identity);    // 폭발 이펙트 생성
        ExpDamage(transform.position);
        //rb.mass = 0.1f;
        //rb.AddForce((Camera.main.transform.forward + Vector3.up) * expFoce);
        StartCoroutine(Respawn());
    }

    /// <summary>
    /// 폭발 시 주변 에너미, 플레이어에게 폭발힘주는 메소드
    /// </summary>
    /// <param name="pos">폭발 원점이 될 위치</param>
    private void ExpDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, expLayer);
        foreach(var coll in colls)
        {
            if(coll.CompareTag("Player"))
            {
                Rigidbody _rb = coll.GetComponent<Rigidbody>();
                _rb.AddExplosionForce(expFoce, pos, expRadius);
                coll.transform.SendMessage("MinusHP", 3);
            }
            else
                coll.transform.SendMessage("MinusHP", 5);
        }
    }

    IEnumerator Respawn()
    {
        renderer.enabled = false;
        collider.enabled = false;
        yield return new WaitForSeconds(30f);
        hitCount = 0;
        transform.position = originPos;
        transform.rotation = originRot;
        renderer.enabled = true;
        collider.enabled = true;
    }
}
