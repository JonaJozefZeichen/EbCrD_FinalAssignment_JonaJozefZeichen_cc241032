using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HudController : MonoBehaviour
{
    private Label counterLabel;
    private Label healthLabel;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        counterLabel = root.Q<Label>("meteorite-counter");
        healthLabel = root.Q<Label>("planet-health");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDestroyedMeteoriteCountChanged += UpdateCounterLabel;
            UpdateCounterLabel(GameManager.Instance.DestroyedMeteoriteCount);
        }

        if (PlanetHealth.Instance != null)
        {
            PlanetHealth.Instance.OnHealthChanged += UpdateHealthLabel;
            UpdateHealthLabel(PlanetHealth.Instance.CurrentHealth, PlanetHealth.Instance.MaxHealth);
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDestroyedMeteoriteCountChanged -= UpdateCounterLabel;
        }

        if (PlanetHealth.Instance != null)
        {
            PlanetHealth.Instance.OnHealthChanged -= UpdateHealthLabel;
        }
    }

    private void UpdateCounterLabel(int count)
    {
        if (counterLabel != null)
        {
            counterLabel.text = $"Destroyed: {count}";
        }
    }

    private void UpdateHealthLabel(int current, int max)
    {
        if (healthLabel != null)
        {
            healthLabel.text = $"Planet HP: {current}/{max}";
        }
    }
}
