using UnityEngine;

public class ContainerZone : MonoBehaviour
{
    public TrashType acceptedType;
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrashObject trash = other.GetComponent<TrashObject>();

        if (trash != null)
        {
            if (trash.type == acceptedType)
            {
                gameManager.AddTime(-0.5f);
            }
            else
            {
                gameManager.AddTime(1f);
            }

            Destroy(other.gameObject);
        }
    }
}