using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float hours = 60f;
    public TextMeshProUGUI hoursText;

    void Start()
    {
        UpdateUI();
    }

    public void AddTime(float amount)
    {
        hours += amount;

        if (hours < 0)
            hours = 0;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (hoursText == null)
        {
            Debug.LogWarning("GameManager: falta asignar el texto de horas.");
            return;
        }

        hoursText.text = hours.ToString("0.0") + " horas";
    }
}