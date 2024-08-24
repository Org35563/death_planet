public interface IEnemy
{
    public int GetHealth();

    public void SetHealth(int newHealthValue);

    public void SetAttack(bool isUnderAttack);
}