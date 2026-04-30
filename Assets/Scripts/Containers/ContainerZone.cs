using UnityEngine;

public class ContainerZone : MonoBehaviour
{
    [Header("Tipo aceptado")]
    public TrashType acceptedType;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Horas")]
    public float correctAmount = -0.5f;
    public float wrongAmount = 1f;

    [Header("Feedback visual")]
    public GameObject correctFeedbackPrefab; // sprite verde -0.5 h
    public GameObject wrongFeedbackPrefab;   // sprite rojo +1 h
    public Transform feedbackPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrashObject trash = other.GetComponentInParent<TrashObject>();

        if (trash == null) return;

        if (gameManager == null)
        {
            Debug.LogWarning("ContainerZone: falta asignar GameManager.");
            return;
        }

        if (trash.type == acceptedType)
        {
            gameManager.AddTime(correctAmount);
            ShowFeedback(correctFeedbackPrefab);
        }
        else
        {
            gameManager.AddTime(wrongAmount);
            ShowFeedback(wrongFeedbackPrefab);
        }

        Destroy(trash.gameObject);
    }

    void ShowFeedback(GameObject prefab)
    {
        if (prefab == null) return;

        Vector3 spawnPosition = transform.position;

        if (feedbackPoint != null)
            spawnPosition = feedbackPoint.position;

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}