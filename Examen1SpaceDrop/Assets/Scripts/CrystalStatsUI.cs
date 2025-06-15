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

    // Método para resetear las estadísticas
    public void ResetStats()
    {
        yellowCrystalsDestroyed = 0;
        blueCrystalsDestroyed = 0;
        redCrystalsDestroyed = 0;
        greenCrystalsDestroyed = 0;
        totalPointsFromCrystals = 0;
        UpdateUI();
    }

    // Métodos para obtener estadísticas
    public int GetCrystalCount(CrystalType type)
    {
        switch (type)
        {
            case CrystalType.Yellow: return yellowCrystalsDestroyed;
            case CrystalType.Blue: return blueCrystalsDestroyed;
            case CrystalType.Red: return redCrystalsDestroyed;
            case CrystalType.Green: return greenCrystalsDestroyed;
            default: return 0;
        }
    }

    public int GetTotalCrystals()
    {
        return yellowCrystalsDestroyed + blueCrystalsDestroyed + redCrystalsDestroyed + greenCrystalsDestroyed;
    }

    public int GetTotalPointsFromCrystals()
    {
        return totalPointsFromCrystals;
    }

    // Método para mostrar un resumen en la consola
    [ContextMenu("Show Crystal Stats")]
    public void ShowStats()
    {
        Debug.Log("=== ESTADÍSTICAS DE CRISTALES ===");
        Debug.Log($"Cristales Amarillos destruidos: {yellowCrystalsDestroyed}");
        Debug.Log($"Cristales Azules destruidos: {blueCrystalsDestroyed}");
        Debug.Log($"Cristales Rojos destruidos: {redCrystalsDestroyed}");
        Debug.Log($"Cristales Verdes destruidos: {greenCrystalsDestroyed}");
        Debug.Log($"Total de cristales: {GetTotalCrystals()}");
        Debug.Log($"Total de puntos por cristales: {totalPointsFromCrystals}");
        Debug.Log("================================");
    }
}
