using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] float lifeTime = 5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
