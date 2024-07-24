using System;
using Unity.Entities;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BotSpawnSystem))]
partial class InputSystem : SystemBase
{
	private PlayerInputSettings playerInputSettings;
	private Entity player;
	public static bool PlayerInputReceived { get; private set; }

	protected override void OnCreate()
	{
		RequireForUpdate<PlayerComponent>();
		playerInputSettings = new PlayerInputSettings();
	}

	protected override void OnStartRunning()
	{
		player = SystemAPI.GetSingletonEntity<PlayerComponent>();
		playerInputSettings.Enable();
		playerInputSettings.Player.Move.performed += OnMove;
		playerInputSettings.Player.TurnLeft.performed += OnTurnLeft;
		playerInputSettings.Player.TurnRight.performed += OnTurnRight;
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

	protected override  void OnStopRunning()
	{
		playerInputSettings.Disable();
		playerInputSettings.Player.Move.performed -= OnMove;
		playerInputSettings.Player.TurnLeft.performed -= OnTurnLeft;
		playerInputSettings.Player.TurnRight.performed -= OnTurnRight;
	}

	protected override void OnUpdate()
	{
		PlayerInputReceived = false;
	}

	private void InputReceived(Actions action)
	{
		if(!TurnSystem.IsTurnFinished)
			return;
		TurnSystem.IsTurnFinished = false;
		SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent(){Action = action});
		PlayerInputReceived = true;
	}
}
