using UnityEngine;
using System;
using System.Collections.Generic;

public class Registries : MonoBehaviour
{   
    private List<EnemyController> m_enemies = new List<EnemyController>();
    private List<LightToCheck> m_lights  = new List<LightToCheck>();
    private List<Collider> m_grapplePoints = new List<Collider>();

    public static Registries Instance;
    public static event Action OnReady;

    private void Awake()
    {
        Instance = this;
        OnReady?.Invoke();
    }

    public void EnemyRegister(EnemyController enemy)
    {
        m_enemies.Add(enemy);
    }

    public void LightRegister(LightToCheck light)
    {
        m_lights.Add(light);
    }

    public void GrapplePointRegister(Collider point)
    {
        m_grapplePoints.Add(point);
    }

    public List<EnemyController> GetEnemies()
    {
        return m_enemies;
    }

    public List<LightToCheck> GetLights()
    {
        return m_lights;
    }

    public List<Collider> GetGrapplePoints()
    {
        return m_grapplePoints;
    }
}
