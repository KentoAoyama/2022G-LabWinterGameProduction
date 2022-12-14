/// <summary>
/// Enemyに継承させる、RetainedHolderBehaviorが継承されたクラス
/// </summary>
public abstract class RetainedEnemyBehavior : RetainedHolderBehavior
{
    public abstract int Id { get; }
            
    public abstract int Health { get; set; }
}
