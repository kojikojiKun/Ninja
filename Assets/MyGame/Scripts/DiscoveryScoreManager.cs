using UnityEngine;
using System;

public class DiscoveryScoreManager : MonoBehaviour
{
    private EnemyController m_highScoreEnemy;
    public event Action<EnemyController> OnUpdateHighScoreEnemy;

    public void NotifyEnemyScoreChanged(EnemyController enemy)
    {
        //プレイヤー発見スコアが最も高い敵が更新されたときのみ実行.
        if (m_highScoreEnemy == null || enemy.GetTotalScore() > m_highScoreEnemy.GetTotalScore())
        {
            m_highScoreEnemy = enemy;
            OnUpdateHighScoreEnemy?.Invoke(enemy);
        }
    }
}
