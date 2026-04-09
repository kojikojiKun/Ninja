using UnityEngine;

public class PlayerCombat
{
    public void Attack()
    {

    }

    public void Assassinate(IAssassinateable target, bool isSee)
    {
        if (isSee)
            target.BeAssassinate();
    }
}
