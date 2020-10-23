using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] float spawnInterval = 3f;

    EnemySpawner[] spawners;

    float timer = 0f;

    void Start()
    {
        spawners = GetComponentsInChildren<EnemySpawner>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(spawnInterval < timer)
        {
            var index = Random.Range(0, spawners.Length);
            //spawners[index].Spawn();
            timer = 0f;
        }
    }
}
