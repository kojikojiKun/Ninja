using UnityEngine;
using System;

[System.Serializable]
public class SpawnLightData
{
    public LightToCheck Light;
    public Transform[] Transforms;
}

[System.Serializable]
public class SpawnEnemyData
{
    public EnemyController enemy;
    public Transform[] Transforms;
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private SpawnLightData[] lightDatas;
    [SerializeField] private SpawnEnemyData[] enemyDatas;
    public event Action<LightToCheck> OnSpawnedLight;
    public event Action<EnemyController> OnSpawnedEnemy;

    private void Start()
    {
        foreach (var data in lightDatas)
        {
            foreach (var pos in data.Transforms)
            {
                LightToCheck light = Instantiate(data.Light, pos.position, Quaternion.identity, this.transform);
                OnSpawnedLight?.Invoke(light);
            }
        }

        foreach (var data in enemyDatas)
        {
            foreach (var pos in data.Transforms)
            {
                EnemyController enemy = Instantiate(data.enemy, pos.position, Quaternion.identity, this.transform);
                OnSpawnedEnemy?.Invoke(enemy);
            }
        }
    }
}
