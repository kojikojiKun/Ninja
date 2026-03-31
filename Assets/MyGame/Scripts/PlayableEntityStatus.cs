public class PlayableEntityStatus {
    public int MaxHp;
    public int Hp;
    public float WalkSpeed;
    public float RunSpeed;
    public float CrouchWalkSpeed;
    public float JumpForce;
    public float SharpnessToTargetSpeed;
    public float TurnSpeed;

    public PlayableEntityStatus(PlayableEntityData data)
    {
        MaxHp = data.MaxHp;
        Hp=data.MaxHp;
        WalkSpeed = data.WalkSpeed;
        RunSpeed = data.RunSpeed;
        CrouchWalkSpeed = data.CrouchWalkSpeed;
        JumpForce = data.JumpForce;
        SharpnessToTargetSpeed = data.SharpnessToTargetSpeed;
        TurnSpeed = data.TurnSpeed;
    }
}
