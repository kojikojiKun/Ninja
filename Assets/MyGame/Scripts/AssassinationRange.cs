using UnityEngine;
using System.Collections.Generic;

public class AssassinationRange : MonoBehaviour
{
    private List<IAssassinateable> m_enemiesWithinRange = new List<IAssassinateable>();

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

    //Colliderì‡ÇÃç≈Ç‡ãﬂÇ¢ìGÇï‘Ç∑.
    public IAssassinateable GetClosest(Transform playerPos)
    {
        if (m_enemiesWithinRange.Count == 0)
            return null;

        IAssassinateable closest = null;
        float minDistance = Mathf.Infinity;

        foreach(var enemy in m_enemiesWithinRange)
        {
            float sqrDistance = (enemy.Transform.position - playerPos.position).sqrMagnitude;

            if (sqrDistance < minDistance)
            {
                minDistance = sqrDistance;
                closest = enemy;
            }
        }

        return closest;
    }
}
