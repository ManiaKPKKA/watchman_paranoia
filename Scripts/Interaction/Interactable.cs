using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactText = "�����������������";
    public AudioClip interactSound;

    public virtual void Interact(GameObject player)
    {
        // ����������� ����
        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position);
        }
    }
}