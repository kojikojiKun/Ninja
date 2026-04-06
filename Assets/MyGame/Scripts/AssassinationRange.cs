using UnityEngine;
using System.Collections.Generic;

public class AssassinationRange : MonoBehaviour
{
    private List<IAssassinateable> m_enemiesWithinRange = new List<IAssassinateable>();
    private ViewTarget m_viewTarget;
    private Timer m_timer;

    private void Awake()
    {
        m_timer = new Timer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAssassinateable>(out var enemy))
        {
            m_enemiesWithinRange.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IAssassinateable>(out var enemy))
        {
            m_enemiesWithinRange.Remove(enemy);
        }
    }

    public void GetView(ViewTarget view)
    {
        m_viewTarget = view;
    }

    //Colliderì‡ÇÃç≈Ç‡ãﬂÇ¢ìGÇï‘Ç∑.
    public IAssassinateable ClosestEnemy(Transform playerPos)
    {
        if (m_enemiesWithinRange.Count == 0 || m_viewTarget==null)
            return null;

        IAssassinateable closest = null;
        float minDistance = Mathf.Infinity;

        foreach(var enemy in m_enemiesWithinRange)
        {
            float distance = Vector3.Distance(enemy.Transform.position, playerPos.position);

            if (distance < minDistance)
            {
                closest = enemy;
            }
        }

        return closest;
    }
}
