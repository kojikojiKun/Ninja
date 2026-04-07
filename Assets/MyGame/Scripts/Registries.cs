using UnityEngine;
using System.Collections.Generic;

public class Registries : MonoBehaviour
{
    public static Registries Instance { get; private set; }
    public HashSet<EnemyController> Enemies { get; private set; } = new HashSet<EnemyController>();
    public HashSet<LightToCheck> Lights { get; private set; } = new HashSet<LightToCheck>();

    private void Awake()
    {
        Instance = this;
    }

    public void EnemyRegister(EnemyController enemy)
    {
        Enemies.Add(enemy);
    }

    public void LightRegister(LightToCheck light)
    {
        Lights.Add(light);
    }

    public void EnemyUnRegister(EnemyController enemy)
    {
        Enemies.Remove(enemy);
    }

    public void LightUnRegister(LightToCheck light)
    {
        Lights.Remove(light);
    }
}
