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
		playerInputSettings = GameController.PlayerInputSettings;
	}
	protected override void OnStartRunning()
	{
		player = SystemAPI.GetSingletonEntity<PlayerComponent>();
		playerInputSettings.Enable();
		playerInputSettings.Player.Move.performed += OnMove;
		playerInputSettings.Player.TurnLeft.performed += OnTurnLeft;
		playerInputSettings.Player.TurnRight.performed += OnTurnRight;
		playerInputSettings.Player.Rest.performed += OnRest;
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
	
	private void OnRest(InputAction.CallbackContext obj)
	{
		InputReceived(Actions.Rest);
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
		var staminaComponent = SystemAPI.GetComponent<StaminaComponent>(player);
		
		if(staminaComponent.HasStamina()) 
			SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent()
				{CurrentAction = action});
		else
		
			SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent()
				{CurrentAction = Actions.Rest});
		
		GameController.PlayerInputReceived = true;
	}

	protected override void OnUpdate() { }
}
