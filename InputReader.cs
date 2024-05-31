using KBCore.Refs;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//���� ���� �ڵ带 �ۼ��ϸ�, MyPlayerController ��ũ��Ʈ�� ���� ������Ʈ�� �߰��� �� Unity�� �ڵ����� PlayerInput ������Ʈ�� �߰�
//���� �ش� ���� ������Ʈ�� �̹� PlayerInput ������Ʈ�� �ִٸ�, ���ο� ������Ʈ�� �߰����� ����.
[RequireComponent(typeof(PlayerInput))]
public class InputReader : ValidatedMonoBehaviour
{
    // Self��� Ŀ���� �Ӽ��� playerInput �ʵ尡 ���� ���� ������Ʈ�� �ִ� PlayerInput ������Ʈ�� �ڵ����� �Ҵ�
    [SerializeField, Self] PlayerInput playerInput;
    [SerializeField] float doubleTapTime = 0.5f; // �� ���̿� �ʿ��� �ð�

    InputAction moveAction;
    InputAction aimAction;
    InputAction fireAction;

    float lastMoveTime; // ���������� �̵� �׼��� ���� �ð�
    float lastMoveDirection; //���������� � ������ ���ȴ��� 

    public event Action LeftTap;
    public event Action RightTap;
    public event Action Fire;

    public Vector2 Move => moveAction.ReadValue<Vector2>();
    public Vector2 Aim => aimAction.ReadValue<Vector2>();

    void Awake()
    {
        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        fireAction = playerInput.actions["Fire"];
    }

    void OnEnable()
    {
        moveAction.performed += OnMovePerformed;
        fireAction.performed += OnFire;
    }

    void OnDisable()
    {
        moveAction.performed -= OnMovePerformed;
        fireAction.performed -= OnFire;
    }

    void OnFire(InputAction.CallbackContext ctx) => Fire?.Invoke();

    //Unity Input System���� �÷��̾ ������ �Է��� ���� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        float currentDirection = Move.x;

        if (Time.time - lastMoveTime < doubleTapTime && currentDirection == lastMoveDirection)
        {
            if (currentDirection < 0)
            {
                LeftTap?.Invoke();
            }
            else if (currentDirection > 0)
            {
                RightTap?.Invoke();
            }
        }

        lastMoveTime = Time.time;
        lastMoveDirection = currentDirection;
    }
}