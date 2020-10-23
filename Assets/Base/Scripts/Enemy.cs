using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip spawnClip;
    [SerializeField] AudioClip hitClip;

    [SerializeField] Collider enemyCollider;
    [SerializeField] Renderer enemyRenederer;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(spawnClip);
    }

    void OnHitBullet()
    {
        audioSource.PlayOneShot(hitClip);

        enemyCollider.enabled = false;
        enemyRenederer.enabled = false;

        Destroy(gameObject, 1f);
    }
}
