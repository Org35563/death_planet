using System.Collections.Generic;
using Godot;

public partial class BackToHomeState : State
{
    [Export]
    public AnimatedSprite2D AnimationPlayer;

    [Export]
    public CharacterBody2D Character;

    [Export]
    public RayCast2D RayCast;

    [Export]
    public string HomeAreaName;

    private CollisionShape2D _homeAreaShape;

    private Area2D _homeArea;

    private Dictionary<string, Vector2> _chasesRayCastDict;

    private float _rayCastLength = 20.0f;

    private float _moveSpeed;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _moveSpeed = combatCreature.GetMoveSpeed();
        }

        _chasesRayCastDict = new ()
        {
            { DirectionNames.RIGHT, new Vector2(-_rayCastLength, 0) },
            { DirectionNames.LEFT, new Vector2(_rayCastLength ,0) },
            { DirectionNames.DOWN, new Vector2(0, _rayCastLength) },
            { DirectionNames.UP, new Vector2(0, -_rayCastLength) },  
        };

        _homeArea = GetNode("../../../").GetNode<Area2D>(HomeAreaName);
        _homeAreaShape = _homeArea.GetChild<CollisionShape2D>(0);
    }

    public override void PhysicsUpdate(float delta)
    {
        if(!Global.IsCharacterOnArea(_homeArea, Character.Position))
        {
            var direction = (_homeAreaShape.Position - Character.Position).Normalized();

            Character.Position += direction * _moveSpeed * delta;

            var newDirectionName = Global.GetNewMoveDirectionName(direction);

            AnimationPlayer.PlayMoveAnimation(newDirectionName, direction.X < 0);

            // TODO: Добавить избегание препятствий
            SetNewRayCastDirection(newDirectionName);

            Character.MoveAndSlide();
        }
        else
        {
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }

    private void SetNewRayCastDirection(string currentDirectionName)
    {
        if(RayCast != null)
        {
            RayCast.TargetPosition = _chasesRayCastDict[currentDirectionName];
        }
    }
}
