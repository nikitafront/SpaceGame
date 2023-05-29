namespace SpaceGame.architecture.interfaces;

public interface IGameMember
{
    public void Shoot(Control.ControlCollection controls);
    public void FlyBullets(
        Control.ControlCollection controls, 
        List<List<EnemyModel>> allEnemies, 
        List<BonusModel> bonuses = null);
    public void Die(
        Control.ControlCollection controls, 
        List<List<EnemyModel>> allEnemies = null, 
        List<BonusModel> bonuses = null, 
        BulletsDamage damage = 0);
}