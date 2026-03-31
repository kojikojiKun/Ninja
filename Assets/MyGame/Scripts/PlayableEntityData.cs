using UnityEngine;

[CreateAssetMenu(menuName =("Game/EntityData"))]
public class PlayableEntityData : ScriptableObject
{
    public int MaxHp;
    public float WalkSpeed;
    public float RunSpeed;
    public float CrouchWalkSpeed;
    public float JumpForce;
    public float SharpnessToTargetSpeed;
    public float TurnSpeed;
}
