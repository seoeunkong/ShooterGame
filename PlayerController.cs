using DG.Tweening;
using KBCore.Refs;
using UnityEngine;

public class PlayerController : ValidatedMonoBehaviour
{
    [SerializeField, Self] InputReader input;

    [SerializeField] Transform followTarget;
    [SerializeField] Transform aimTarget;

    [SerializeField] Transform playerModel;
    [SerializeField] float followDistance = 2f; // 타겟 뒤에 얼마나 멀리 있어야 하는지 거리
    [SerializeField] Vector2 movementLimit = new Vector2(2f, 2f); // 플레이어가 타겟에서 얼마나 멀리 이동할 수 있는지 제한
    [SerializeField] float movementSpeed = 10f; // 플레이어가 Follow 타겟에서 상하좌우로 얼마나 빠르게 이동할 수 있는지
    [SerializeField] float smoothTime = 0.2f; // 얼마나 빨리 원래 위치로 부드럽게 되돌아올지

    [SerializeField] float maxRoll = 15f; // 회전 범위
    [SerializeField] float rollSpeed = 2f; // 회전 속도
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
        // 타겟 방향 계산
        Vector3 direction = aimTarget.position - transform.position;

        // 타겟을 바라보는 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        modelParent.rotation = Quaternion.Lerp(modelParent.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    void HandlePosition()
    {
        // 타겟 거리 및 타겟 위치를 기반으로 타겟 위치 계산
        Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;

        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // 새로운 로컬 위치 계산
        Vector3 localPos = transform.InverseTransformPoint(smoothedPos);
        localPos.x += -input.Move.x * movementSpeed * Time.deltaTime;
        localPos.y += -input.Move.y * movementSpeed * Time.deltaTime;

        localPos.x = Mathf.Clamp(localPos.x, -movementLimit.x, movementLimit.x);
        localPos.y = Mathf.Clamp(localPos.y, -movementLimit.y, movementLimit.y);

        // 플레이어 위치 업데이트
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
