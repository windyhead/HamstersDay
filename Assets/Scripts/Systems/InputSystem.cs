using Unity.Entities;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup),OrderLast = true)]
partial class InputSystem : SystemBase
{
	private PlayerInputSettings playerInputSettings;
	private Entity player;

	protected override void OnCreate()
	{
		RequireForUpdate<PlayerComponent>();
		playerInputSettings = new PlayerInputSettings();
	}

	protected override  void OnStartRunning()
	{
		var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
		player = SystemAPI.GetSingletonEntity<PlayerComponent>();
		var ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);
		var actionc = new ActionComponent() { Action = Actions.None };
		ecb.AddComponent<ActionComponent>(player,actionc);
		playerInputSettings.Enable();
		playerInputSettings.Player.Move.performed += OnMove;
		playerInputSettings.Player.TurnLeft.performed += OnTurnLeft;
		playerInputSettings.Player.TurnRight.performed += OnTurnRight;
	}

	private void OnTurnRight(InputAction.CallbackContext obj)
	{
		SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent(){Action = Actions.TurnRight});
	}

	private void OnTurnLeft(InputAction.CallbackContext obj)
	{
		SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent(){Action = Actions.TurnLeft});
	}

	private void OnMove(InputAction.CallbackContext obj)
	{
		SystemAPI.SetComponent<ActionComponent>(player,new ActionComponent(){Action = Actions.Move});
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
	}
}
