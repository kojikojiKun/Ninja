using UnityEngine;
public interface IDamageable
{
    public Transform OriginTransform { get; }
    void TakeDamage(int value) { }
}
