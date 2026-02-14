using UnityEngine;

[CreateAssetMenu(menuName =("Game/EnemyData"))]
public class EnemyData : ScriptableObject
{
    public float PatrolSpeed;
    public float SearchTime;
    public float ChaseSpeed;
    public float ViewDistance;
    public float ViewAngle;
}
