using UnityEngine;

public class StaticNPCTrashSpawner : MonoBehaviour
{
    [Header("Basura que puede soltar")]
    public GameObject[] trashPrefabs;

    [Header("Punto donde aparece la basura")]
    public Transform dropPoint;

    [Header("Tiempo")]
    public float dropInterval = 5f;
    public bool randomizeInterval = false;
    public float minInterval = 3f;
    public float maxInterval = 8f;

    private float timer = 0f;
    private float currentInterval;

    void Start()
    {
        SetNextInterval();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentInterval)
        {
            DropTrash();
            timer = 0f;
            SetNextInterval();
        }
    }

    void SetNextInterval()
    {
        if (randomizeInterval)
            currentInterval = Random.Range(minInterval, maxInterval);
        else
            currentInterval = dropInterval;
    }

    void DropTrash()
    {
        if (trashPrefabs == null || trashPrefabs.Length == 0) return;
        if (dropPoint == null) return;

        GameObject selectedTrash = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

        Instantiate(selectedTrash, dropPoint.position, Quaternion.identity);
    }
}