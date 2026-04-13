using UnityEngine;

public class PlayerCombat
{
    private bool m_isSuccess;

    public void Attack()
    {

    }

    public void TryAssassinate(AssassinateContext context)
    {
        if (!context.CanSee)
        {
            m_isSuccess = false;
            return;
        }

        context.Target.BeAssassinate(context.Direction);

        m_isSuccess = true;
    }

    public Transform GetSnapPoint(AssassinateContext context)
    {
        foreach (var dict in context.DataMap)
        {
            if (dict.Value.Direction == context.Direction)
            {
                return dict.Value.SnapPoint;
            }
        }

        return null;
    }

    public bool IsSuccess()
    {
        return m_isSuccess;
    }
}
