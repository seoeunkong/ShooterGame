using KBCore.Refs;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//위와 같은 코드를 작성하면, MyPlayerController 스크립트를 게임 오브젝트에 추가할 때 Unity는 자동으로 PlayerInput 컴포넌트도 추가
//만약 해당 게임 오브젝트에 이미 PlayerInput 컴포넌트가 있다면, 새로운 컴포넌트를 추가하지 않음.
[RequireComponent(typeof(PlayerInput))]
public class InputReader : ValidatedMonoBehaviour
{
    // Self라는 커스텀 속성은 playerInput 필드가 현재 게임 오브젝트에 있는 PlayerInput 컴포넌트를 자동으로 할당
    [SerializeField, Self] PlayerInput playerInput;
    [SerializeField] float doubleTapTime = 0.5f; // 탭 사이에 필요한 시간

    InputAction moveAction;
    InputAction aimAction;
    InputAction fireAction;

    float lastMoveTime; // 마지막으로 이동 액션이 눌린 시간
    float lastMoveDirection; //마지막으로 어떤 방향이 눌렸는지 

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

    //Unity Input System에서 플레이어가 움직임 입력을 했을 때 호출되는 이벤트 핸들러
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