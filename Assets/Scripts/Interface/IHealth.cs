public interface IHealth
{
    int currentHP { get; }
    int maxHP { get; }
    void TakeDamage(int amount);
    void Regenerate(float deltaTime);
}