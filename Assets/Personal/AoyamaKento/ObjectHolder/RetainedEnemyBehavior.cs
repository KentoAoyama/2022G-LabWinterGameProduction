/// <summary>
/// Enemyに継承させる、RetainedHolderBehaviorが継承されたクラス
/// </summary>
public abstract class RetainedEnemyBehavior : RetainedHolderBehavior
{
    protected abstract int Id { get; }
            
    protected abstract int Health { get; set; }
}
