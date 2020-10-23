using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    private void Start()
    {
        Spawn();
    }

    public void Spawn()
    {
        int spawnCount = PlaySceneManager.instance.StageLevel * 10;
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-4, 5), 0, Random.Range(-4, 5)) * 5;
            Instantiate(enemyPrefab, pos, Quaternion.identity, transform);
        }
    }
}
