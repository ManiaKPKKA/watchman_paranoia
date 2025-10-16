using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [Header("��������� ��������������")]
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    public TextMeshProUGUI interactText;

    private Camera playerCamera;
    private Interactable currentInteractable;
    private ItemDragger itemDragger;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        itemDragger = GetComponent<ItemDragger>();

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        FindInteractable();
        CheckForInteractInput();
    }

    void FindInteractable()
    {
        // ���� ������������� ������� - �� ���� ������ ��������������
        if (itemDragger != null && itemDragger.IsDragging())
        {
            if (interactText != null)
                interactText.gameObject.SetActive(false);
            currentInteractable = null;
            return;

        }

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Interactable newInteractable = hit.collider.gameObject.GetComponent<Interactable>();

            if (newInteractable != null)
            {
                currentInteractable = newInteractable;
                if (interactText != null)
                {
                    interactText.text = "[E] " + currentInteractable.interactText;
                    interactText.gameObject.SetActive(true);
                }
                return;
            }
        }

        // ���� ������ �� �����
        currentInteractable = null;
        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    void CheckForInteractInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }
}