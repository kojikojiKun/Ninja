using UnityEngine;

public interface IAssassinateable
{
    Transform Transform { get; }

    void Assasinate() { }
}
