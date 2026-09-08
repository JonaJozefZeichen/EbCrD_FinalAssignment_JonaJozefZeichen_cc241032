using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameFlowController : MonoBehaviour
{
    [Header("Gameplay References")]
    [SerializeField] private SphericalPlayerController playerController;
    [SerializeField] private LaserGun laserGun;
    [SerializeField] private MeteoriteSpawner spawner;

    private VisualElement hudRoot;
    private VisualElement startScreen;
    private VisualElement endScreen;
    private Label finalScoreLabel;

    private void Awake()
    {
        SetGameplayEnabled(false);
    }

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        hudRoot = root.Q<VisualElement>("hud-root");
        startScreen = root.Q<VisualElement>("start-screen");
        endScreen = root.Q<VisualElement>("end-screen");
        finalScoreLabel = root.Q<Label>("final-score-label");

        root.Q<Button>("start-button").clicked += HandleStartClicked;
        root.Q<Button>("restart-button").clicked += HandleRestartClicked;

        if (PlanetHealth.Instance != null)
        {
            PlanetHealth.Instance.OnPlanetDestroyed += HandlePlanetDestroyed;
        }
    }

    private void HandleStartClicked()
    {
        startScreen.style.display = DisplayStyle.None;
        hudRoot.style.display = DisplayStyle.Flex;
        SetGameplayEnabled(true);
    }

    private void HandlePlanetDestroyed()
    {
        SetGameplayEnabled(false);
        hudRoot.style.display = DisplayStyle.None;

        if (finalScoreLabel != null && GameManager.Instance != null)
        {
            finalScoreLabel.text = $"Meteorites Destroyed: {GameManager.Instance.DestroyedMeteoriteCount}";
        }

        endScreen.style.display = DisplayStyle.Flex;
    }

    private void HandleRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetGameplayEnabled(bool isEnabled)
    {
        if (playerController != null) playerController.enabled = isEnabled;
        if (laserGun != null) laserGun.enabled = isEnabled;
        if (spawner != null) spawner.enabled = isEnabled;
    }
}
