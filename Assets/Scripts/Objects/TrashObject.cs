using UnityEngine;

public enum TrashType
{
    Organico,
    Plastico,
    Vidrio,
    Carton
}

public class TrashObject : MonoBehaviour
{
    public TrashType type;
}