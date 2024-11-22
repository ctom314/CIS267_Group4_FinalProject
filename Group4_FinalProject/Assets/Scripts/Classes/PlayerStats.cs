/// <summary>
/// Used to store player stats
/// </summary>

public class PlayerStats
{
    // Defaults
    public const int DEFAULT_HEALTH = 3;

    private int health;
    private int maxHealth;
    // TODO: Add more stats

    // Constructor
    public PlayerStats(int hp, int maxHp)
    {
        health = hp;
        maxHealth = maxHp;
    }

    // Getters
    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    // Setters
    public void SetHealth(int hp)
    {
        health = hp;
    }

    public void SetMaxHealth(int maxHp)
    {
        maxHealth = maxHp;
    }
}