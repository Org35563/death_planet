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

    private float _rayLength = 20.0f;

    private float _moveSpeed;

    public override void _Ready()
    {
        if(Character is ICombatCreature combatCreature)
        {
            _moveSpeed = combatCreature.GetMoveSpeed();
        }

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
            RayCast.SetNewRayCastDirection(newDirectionName, _rayLength);

            Character.MoveAndSlide();
        }
        else
        {
            StateMachine.TransitionTo(StateNames.Wander);
        }
    }
}
