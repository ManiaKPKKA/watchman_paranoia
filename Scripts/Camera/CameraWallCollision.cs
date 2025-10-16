using UnityEngine;

public class CameraWallCollision : MonoBehaviour
{
    [Header("��������� ������������")]
    public float clipOffset = 0.1f;
    public float minCameraDistance = 0.5f;
    public LayerMask collisionLayer = 1; // �� ��������� ��� ����

    private Vector3 originalLocalPos;
    private Transform playerTransform;

    void Start()
    {
        // ���������� ������������ ������� ������ ������������ ������
        originalLocalPos = transform.localPosition;
        playerTransform = transform.parent;
    }

    void LateUpdate()
    {
        HandleCameraCollision();
    }

    void HandleCameraCollision()
    {
        // �arget ������� ������ (��� ��� ������ ���� ��� ������������)
        Vector3 targetPos = playerTransform.TransformPoint(originalLocalPos);
        Vector3 direction = targetPos - playerTransform.position;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(playerTransform.position, direction, out hit, distance + clipOffset, collisionLayer))
        {
            // ������������ - ���������� ������ ����� � ������
            float newDistance = Mathf.Max(hit.distance - clipOffset, minCameraDistance);
            transform.position = playerTransform.position + direction.normalized * newDistance;
        }
        else
        {
            // ��� ������������ - ���������� ������ �� ���������� �������
            transform.localPosition = originalLocalPos;
        }
    }
}