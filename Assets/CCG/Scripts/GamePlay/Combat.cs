using UnityEngine;
using Mirror;

public class Combat : NetworkBehaviour
{
    [Header("Entity")]
    public Entity entity;

    [Command(ignoreAuthority = true)]
    public void CmdChangeMana(int amount)
    {
        // Increase mana by amount. If 3, increase by 3. If -3, reduce by 3.
        if (entity is Player) entity.GetComponent<Player>().mana += amount;
    }

    [Command(ignoreAuthority = true)]
    public void CmdChangeStrength(int amount)
    {
   
        entity.strength += amount;
    }

    [Command(ignoreAuthority = true)]
    public void CmdChangeHealth(int amount)
    {
   
        entity.health += amount;
        if (entity.health <= 0 && !(entity is Player))
        {
            Destroy(entity.gameObject);
        }
            
    }

    [Command(ignoreAuthority = true)]
    public void CmdIncreaseWaitTurn()
    {
        entity.waitTurn++;
    }
}
