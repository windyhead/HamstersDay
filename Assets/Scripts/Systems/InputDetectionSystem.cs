using Unity.Entities;
using UnityEngine.InputSystem;

[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial class InputDetectionSystem : SystemBase
{
	private PlayerInputSettings playerInputSettings;
	private Entity player;

	protected override void OnCreate()
	{
		RequireForUpdate<PlayerComponent>();
		playerInputSettings = new PlayerInputSettings();
	}

	protected override void OnStartRunning()
	{
		SetPlayer();
	}

	private void OnTurnRight(InputAction.CallbackContext obj)
	{
		InputReceived(Actions.TurnRight);
	}

	private void OnTurnLeft(InputAction.CallbackContext obj)
	{
		InputReceived(Actions.TurnLeft);
	}

	private void OnMove(InputAction.CallbackContext obj)
	{
		InputReceived(Actions.Move);
	}

	protected override void OnStopRunning()
	{
		playerInputSettings.Disable();
		playerInputSettings.Player.Move.performed -= OnMove;
		playerInputSettings.Player.TurnLeft.performed -= OnTurnLeft;
		playerInputSettings.Player.TurnRight.performed -= OnTurnRight;
	}
	
	private void InputReceived(Actions action)
	{
		if(!GameController.IsTurnFinished)
			return;
		GameController.IsTurnFinished = false;
		SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent(){Action = action});
		GameController.PlayerInputReceived = true;
	}

	protected override void OnUpdate()
	{
		
	}

	private void SetPlayer()
	{
		player = SystemAPI.GetSingletonEntity<PlayerComponent>();
		playerInputSettings.Enable();
		playerInputSettings.Player.Move.performed += OnMove;
		playerInputSettings.Player.TurnLeft.performed += OnTurnLeft;
		playerInputSettings.Player.TurnRight.performed += OnTurnRight;
	}
}
