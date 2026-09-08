using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int DestroyedMeteoriteCount { get; private set; }

    public event Action<int> OnDestroyedMeteoriteCountChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterMeteoriteDestroyed()
    {
        DestroyedMeteoriteCount++;
        OnDestroyedMeteoriteCountChanged?.Invoke(DestroyedMeteoriteCount);
    }
}
