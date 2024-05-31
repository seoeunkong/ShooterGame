using KBCore.Refs;
using UnityEngine;

public class WeaponSystem : ValidatedMonoBehaviour
{
    [SerializeField, Self] InputReader input;

    [SerializeField] Transform targetPoint;
    [SerializeField] float targetDistance = 50f;
    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] Vector2 aimLimit = new Vector2(50f, 20f);
    [SerializeField] float aimSpeed = 10f;
    [SerializeField] float aimReturnSpeed = 0.2f; // �÷��̾ ������ ������ ��ƽ�̳� ���콺�� �������� ���� �� �󸶳� ������ �߸����� ���ƿ��⸦ ���ϴ���

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;

    Vector3 velocity;
    Vector2 aimOffset;

    void Awake()
    {
        input.Fire += OnFire;
    }

    void Start()
    {
        UnityEngine.Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // �÷��̾��� ���� ��ġ���� targetDistance��ŭ �տ� targetPosition�� ����
        Vector3 targetPosition = transform.position + transform.forward * targetDistance;
        Vector3 localPos = transform.InverseTransformPoint(targetPosition);

        // ���� �Է��� �ִ� ���
        if (input.Aim != Vector2.zero)
        {
            // ���� �Է��� �߰�
            aimOffset += input.Aim * aimSpeed * Time.deltaTime;

            aimOffset.x = Mathf.Clamp(aimOffset.x, -aimLimit.x, aimLimit.x);
            aimOffset.y = Mathf.Clamp(aimOffset.y, -aimLimit.y, aimLimit.y);
        }
        else
        {
            aimOffset = Vector2.Lerp(aimOffset, Vector2.zero, Time.deltaTime * aimReturnSpeed);
        }

        // ���� ��ġ�� ����
        localPos.x += aimOffset.x;
        localPos.y += aimOffset.y;

        var desiredPosition = transform.TransformPoint(localPos);

        targetPoint.position = Vector3.SmoothDamp(targetPoint.position, desiredPosition, ref velocity, smoothTime);
    }

    void OnFire()
    {
        var projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(targetPoint.position - firePoint.position));
        Destroy(projectile, 5f);
    }

    void OnDestroy()
    {
        input.Fire -= OnFire;
    }
}
