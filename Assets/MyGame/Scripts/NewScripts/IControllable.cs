using UnityEngine;
public interface IControllable {
    void Move(Vector2 input,Transform cam) { }
    void SetTargetSpeed(float speed) { }
    void Jump() { }
}
