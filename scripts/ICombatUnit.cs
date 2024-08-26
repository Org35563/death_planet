public interface ICombatUnit
{
    public int GetHealth();

    public void SetHealth(int newHealthValue);

    public void SetIsUnderAttack(bool isUnderAttack);
}