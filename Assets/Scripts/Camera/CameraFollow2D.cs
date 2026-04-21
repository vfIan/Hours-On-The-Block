using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public float posX = 0f;
    public float posY = 0f;
    private float fixedZ = -10f;

    void Start()
    {
        transform.position = new Vector3(posX, posY, fixedZ);
    }
}