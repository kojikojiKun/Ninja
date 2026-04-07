using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] PlayerContoller m_player;
    [SerializeField] Camera m_camera;

    public void Initialize(HashSet<EnemyController> enemies,HashSet<LightToCheck> checks)
    {
        foreach (var enemy in enemies)
        {
            enemy.Initialize(checks,m_player);
        }

        m_player.Initialize(m_camera);
    }
}
