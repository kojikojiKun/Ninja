using UnityEngine;

[CreateAssetMenu(menuName =("Game/EnemyData"))]
public class EnemyData : ScriptableObject
{
    public int MaxHp;
    [Range(0, 99.9999f)] public float CautionScorePercentage;
    public float PatrolSpeed;
    public float SearchTime;
    public float ChaseSpeed;
}
