using DG.Tweening;
using KBCore.Refs;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [SerializeField, Self] InputReader input;

    [SerializeField] Transform followTarget;
    [SerializeField] Transform aimTarget;

    [SerializeField] Transform playerModel;
    [SerializeField] float followDistance = 2f; // Ÿ�� �ڿ� �󸶳� �ָ� �־�� �ϴ��� �Ÿ�
    [SerializeField] Vector2 movementLimit = new Vector2(2f, 2f); // �÷��̾ Ÿ�ٿ��� �󸶳� �ָ� �̵��� �� �ִ��� ����
    [SerializeField] float movementSpeed = 10f; // �÷��̾ Follow Ÿ�ٿ��� �����¿�� �󸶳� ������ �̵��� �� �ִ���
    [SerializeField] float smoothTime = 0.2f; // �󸶳� ���� ���� ��ġ�� �ε巴�� �ǵ��ƿ���

    [SerializeField] float maxRoll = 15f; // ȸ�� ����
    [SerializeField] float rollSpeed = 2f; // ȸ�� �ӵ�
    [SerializeField] float rollDuration = 1f;

    [SerializeField] Transform modelParent;
    [SerializeField] float rotationSpeed = 5f;

    Vector3 velocity;
    float roll;

    void Awake()
    {
        input.LeftTap += OnLeftTap;
        input.RightTap += OnRightTap;
    }

    void Update()
    {
        HandlePosition();
        HandleRoll();
        HandleRotation();
    }

    void HandleRotation()
    {
        // Ÿ�� ���� ���
        Vector3 direction = aimTarget.position - transform.position;

        // Ÿ���� �ٶ󺸴� ȸ�� ���
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        modelParent.rotation = Quaternion.Lerp(modelParent.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void HandlePosition()
    {
        // Ÿ�� �Ÿ� �� Ÿ�� ��ġ�� ������� Ÿ�� ��ġ ���
        Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;

        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // ���ο� ���� ��ġ ���
        Vector3 localPos = transform.InverseTransformPoint(smoothedPos);
        localPos.x += -input.Move.x * movementSpeed * Time.deltaTime;
        localPos.y += -input.Move.y * movementSpeed * Time.deltaTime;

        localPos.x = Mathf.Clamp(localPos.x, -movementLimit.x, movementLimit.x);
        localPos.y = Mathf.Clamp(localPos.y, -movementLimit.y, movementLimit.y);

        // �÷��̾� ��ġ ������Ʈ
        transform.position = transform.TransformPoint(localPos);
    }

    void HandleRoll()
    {
        transform.rotation = followTarget.rotation;

        roll = Mathf.Lerp(roll, -input.Move.x * maxRoll, Time.deltaTime * rollSpeed);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, roll);
    }

    void OnLeftTap()
    {
        BarrelRoll();
    }

    void OnRightTap() => BarrelRoll(-1);

    void BarrelRoll(int direction = 1)
    {
        if (!DOTween.IsTweening(playerModel))
        {
            playerModel.DOLocalRotate(new Vector3(
                playerModel.localEulerAngles.x,
                playerModel.localEulerAngles.y,
                360 * direction), rollDuration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.OutCubic);
        }
    }

    void OnDestroy()
    {
        input.LeftTap -= OnLeftTap;
        input.RightTap -= OnRightTap;
    }
}
