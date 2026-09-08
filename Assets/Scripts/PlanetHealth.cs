using System;
using UnityEngine;

public class PlanetHealth : MonoBehaviour
{
    public static PlanetHealth Instance { get; private set; }

    [SerializeField] private int maxHealth = 100;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;

    public event Action<int, int> OnHealthChanged;
    public event Action OnPlanetDestroyed;

    private bool isDestroyed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (CurrentHealth <= 0)
        {
            isDestroyed = true;
            OnPlanetDestroyed?.Invoke();
        }
    }
}
