using System.Linq;
using Godot;

public static class Global
{
    public static bool HeroCurrentAttack = false;

    public static int HeroAttackValue = 5;

    public static int EnemyAttackValue = 5;

    public static bool IsEnemy(Node2D body)
    {
      var bodyInterfaces = body
        .GetType()
        .GetInterfaces();

      return bodyInterfaces.Contains(typeof(IEnemy));
    }
}