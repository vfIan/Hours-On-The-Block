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
        }
        else
        {
            gameManager.AddTime(wrongAmount);
        }

        Destroy(trash.gameObject);
    }
}