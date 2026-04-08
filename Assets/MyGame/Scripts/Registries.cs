using UnityEngine;
using System.Collections.Generic;

public class Registries : MonoBehaviour
{
    public List<EnemyController> Enemies { get; private set; } = new List<EnemyController>();
    public List<LightToCheck> Lights { get; private set; } = new List<LightToCheck>();

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
