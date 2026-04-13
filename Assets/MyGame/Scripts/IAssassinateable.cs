using UnityEngine;
using System.Collections.Generic;

public enum AssassinateDirection
{
    Front,
    Back,
    Left,
    Right
}

public class AssassinateContext
{
    public IAssassinateable Target;
    public bool CanSee;
    public AssassinateDirection Direction;
    public Dictionary<AssassinateDirection, AssassinateData> DataMap;
}

[System.Serializable]
public class AssassinateData
{
    public Transform SnapPoint;
    public AssassinateDirection Direction;
}

public interface IAssassinateable
{
    Transform OriginTransform { get;}
    Dictionary<AssassinateDirection, AssassinateData> DataMap { get; }

    void BeAssassinate(AssassinateDirection dir) { }
}
