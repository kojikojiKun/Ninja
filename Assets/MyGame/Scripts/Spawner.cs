using UnityEngine;
using System;

[System.Serializable]
public class SpawnEnemyData
{
    public EnemyController enemy;
    public Transform[] Transforms;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnEnemyData[] enemyDatas;
    private void Awake()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        foreach (var data in enemyDatas)
        {
            foreach (var pos in data.Transforms)
            {
                EnemyController enemy = Instantiate(data.enemy, pos.position, Quaternion.identity, this.transform);
            }
        }
    }
}
