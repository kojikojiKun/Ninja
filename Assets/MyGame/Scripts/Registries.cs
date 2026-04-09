using UnityEngine;
using System.Collections.Generic;

public class Registries : MonoBehaviour
{
    private List<EnemyController> m_enemies = new List<EnemyController>();
    private List<LightToCheck> m_lights  = new List<LightToCheck>();

    public void EnemyRegister(EnemyController enemy)
    {
        m_enemies.Add(enemy);
    }

    public void LightRegister(LightToCheck light)
    {
        m_lights.Add(light);
    }

    public List<EnemyController> GetEnemies()
    {
        return m_enemies;
    }

    public List<LightToCheck> GetLights()
    {
        return m_lights;
    }

    public void EnemyUnRegister(EnemyController enemy)
    {
        m_enemies.Remove(enemy);
    }

    public void LightUnRegister(LightToCheck light)
    {
        m_lights.Remove(light);
    }
}
