using UnityEngine;
public interface IControllable {
    void Move(Vector2 input,Transform cam) { }
    void SetTargetSpeed() { }
    void Jump() { }
    void Attack() { }
    void Assasinate() { }
}
