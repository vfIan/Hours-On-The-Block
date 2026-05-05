using UnityEngine;

public class WorldPowerBar : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;
    public string animationStateName = "PowerBarAnim";

    [Header("Posición respecto al jugador")]
    public float sideOffsetX = 1.2f;
    public float offsetY = 1.5f;

    private bool isVisible = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        isVisible = true;
        SetPower(0f);
    }

    public void Hide()
    {
        isVisible = false;
        gameObject.SetActive(false);
    }

    public void SetPower(float value)
    {
        if (!isVisible) return;
        if (animator == null) return;

        value = Mathf.Clamp01(value);

        animator.Play(animationStateName, 0, value);
        animator.Update(0f);
    }

    public void SetSide(bool facingLeft)
    {
        Vector3 pos = transform.localPosition;

        // Si apunta a la izquierda, la barra sale a la derecha.
        // Si apunta a la derecha, la barra sale a la izquierda.
        pos.x = facingLeft ? sideOffsetX : -sideOffsetX;
        pos.y = offsetY;
        pos.z = 0f;

        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
    }
}