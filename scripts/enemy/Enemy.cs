using Godot;

public partial class Enemy : CharacterBody2D, ICharacter
{
    [Export]
    private AnimatedSprite2D AnimationPlayer;

    private Timer _enemyDeathTimer;

    private string _currentDirection;

    private bool _stopMovement = false;

    private bool _heroChase = false;

    private bool _heroInAttackZone = false;

    private bool _theEnd;

    private bool _enemyAttackCooldownFinished = true;

    private bool _isEnemyAttacking = false;

    private float _speed;

    private int _health;

    private bool _isAlive { get => _health >= 0; }

    private int _attackPower;

    public override void _Ready()
    {
		_health = (int) GetMeta(HeroMetadataNames.Health);
		_attackPower = (int) GetMeta(HeroMetadataNames.AttackPower);
        _speed = (float) GetMeta(HeroMetadataNames.Speed);
        _theEnd = false;

        _enemyDeathTimer = GetNode<Timer>(EnemyNodeNames.DeathTimer);
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckIsEnemyAlive();
    }

    public int GetHealth() => _health;

    public void SetHealth(int newHealthValue) => _health = newHealthValue;

    public void OnEnemyDeathTimerTimeout()
    {
        _enemyDeathTimer.Stop();
        this.QueueFree();
    }

    private void CheckIsEnemyAlive()
    {
        if(!_isAlive && !_theEnd)
        {   
            AnimationPlayer.Play(AnimationNames.DEATH); 
            _enemyDeathTimer.Start();
            _theEnd = true;
        }
    }

    public bool IsAlive() => _isAlive;

    public Vector2 GetCurrentPosition() => Position;
}


