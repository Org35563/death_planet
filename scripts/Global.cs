using System.Linq;
using Godot;

public static class Global
{
    public static bool IsGameUnitType<T>(Node2D body)
    {
      var bodyInterfaces = body
        .GetType()
        .GetInterfaces();

      return bodyInterfaces.Contains(typeof(T));
    }
}