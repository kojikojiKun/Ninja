using UnityEngine;
public interface IControllable {
    void Move(Vector2 input,Transform cam) { }
    void StartRun() { }
    void StopRun() { }
    void Crouch() { }
    void Jump() { }
}
