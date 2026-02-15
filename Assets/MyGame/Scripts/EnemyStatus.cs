public class EnemyStatus {
    public int MaxHp;
    public int Hp;
    public float PatrolSpeed;
    public float SearchTime;
    public float ChaseSpeed;
    public float ViewDistance;
    public float ViewAngle;
    public EnemyStatus(EnemyData data)
    {
        MaxHp = data.MaxHp;
        Hp=data.MaxHp;
        PatrolSpeed = data.PatrolSpeed;
        SearchTime = data.SearchTime;
        ChaseSpeed = data.ChaseSpeed;
        ViewDistance = data.ViewDistance;
        ViewAngle = data.ViewAngle;
    }
}
