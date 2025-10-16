using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactText = "Взаимодействовать";
    public AudioClip interactSound;

    public virtual void Interact(GameObject player)
    {
        // Проигрываем звук
        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position);
        }
    }
}