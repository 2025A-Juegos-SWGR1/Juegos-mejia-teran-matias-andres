using UnityEngine;
using UnityEngine.UI;

public class CrystalStatsUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Text yellowCountText;
    public Text blueCountText;
    public Text redCountText;
    public Text greenCountText;
    public Text totalCrystalsText;
    public Text totalPointsFromCrystalsText;

    [Header("Configuración")]
    public bool enableStatsTracking = true;
    public bool showDebugInfo = false;

    // Estadísticas de cristales
    private int yellowCrystalsDestroyed = 0;
    private int blueCrystalsDestroyed = 0;
    private int redCrystalsDestroyed = 0;
    private int greenCrystalsDestroyed = 0;
    private int totalPointsFromCrystals = 0;

    // Instancia singleton (opcional)
    public static CrystalStatsUI Instance { get; private set; }

    void Awake()
    {
        // Configurar singleton si no existe
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    // Método para registrar la destrucción de un cristal
    public void RegisterCrystalDestroyed(CrystalType type, int points)
    {
        if (!enableStatsTracking) return;

        switch (type)
        {
            case CrystalType.Yellow:
                yellowCrystalsDestroyed++;
                break;
            case CrystalType.Blue:
                blueCrystalsDestroyed++;
                break;
            case CrystalType.Red:
                redCrystalsDestroyed++;
                break;
            case CrystalType.Green:
                greenCrystalsDestroyed++;
                break;
        }

        totalPointsFromCrystals += points;
        UpdateUI();

        if (showDebugInfo)
        {
            Debug.Log($"Cristal {type} destruido. Puntos: {points}. Total de puntos por cristales: {totalPointsFromCrystals}");
        }
    }

    // Método para actualizar la UI
    private void UpdateUI()
    {
        if (yellowCountText != null)
            yellowCountText.text = $"Amarillos: {yellowCrystalsDestroyed}";

        if (blueCountText != null)
            blueCountText.text = $"Azules: {blueCrystalsDestroyed}";

        if (redCountText != null)
            redCountText.text = $"Rojos: {redCrystalsDestroyed}";

        if (greenCountText != null)
            greenCountText.text = $"Verdes: {greenCrystalsDestroyed}";

        if (totalCrystalsText != null)
        {
            int total = yellowCrystalsDestroyed + blueCrystalsDestroyed + redCrystalsDestroyed + greenCrystalsDestroyed;
            totalCrystalsText.text = $"Total: {total}";
        }

        if (totalPointsFromCrystalsText != null)
            totalPointsFromCrystalsText.text = $"Puntos por cristales: {totalPointsFromCrystals}";
    }
}
