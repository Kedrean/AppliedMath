using UnityEngine;

public class PlayerHealth : IHealth
{
    private int _hp;
    private readonly int _maxHP;
    private readonly float _regenRate;
    private float _regenAccumulator = 0f;

    public int currentHP => _hp;
    public int maxHP => _maxHP;

    public delegate void OnHealthChanged(int hp, int max);
    public event OnHealthChanged HealthChanged;

    public PlayerHealth(int maxHP, float regenRate)
    {
        _maxHP = maxHP;
        _hp = maxHP;
        _regenRate = regenRate;
    }

    public void TakeDamage(int amount)
    {
        _hp = Mathf.Max(0, _hp - amount);
        HealthChanged?.Invoke(_hp, _maxHP);
    }

    public void Regenerate(float deltaTime)
    {
        if (_hp >= _maxHP) return;
        _regenAccumulator += _regenRate * deltaTime;
        if (_regenAccumulator >= 1f)
        {
            int heal = Mathf.FloorToInt(_regenAccumulator);
            _hp = Mathf.Min(_maxHP, _hp + heal);
            _regenAccumulator -= heal;
            HealthChanged?.Invoke(_hp, _maxHP);
        }
    }
}
