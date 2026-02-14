using UnityEngine;

[CreateAssetMenu(menuName ="Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public int HP;
    public float WalkSpeed;
    public float RunSpeed;
    public float CrouchWalkSpeed;
    public float JumpForce;

}
