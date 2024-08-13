using System;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(GameOverSystem))]

partial class PopulationSystem : SystemBase
{
	public static Action<int> OnPopulationChanged;
	public static int Population { get; private set; } = 1;
	private static int populationCounter = 0;

	public void OnCreate(ref SystemState state)
	{
	}
	
	protected override void OnStartRunning()
	{
		TurnSystem.OnTurnFinished += IncreasePopulation;
		BotDestroySystem.OnBorDestroyed += LowerPopulation;
	}
	
	protected override void OnStopRunning()
	{
		TurnSystem.OnTurnFinished -= IncreasePopulation;
		BotDestroySystem.OnBorDestroyed -= LowerPopulation;
	}
	
	private void IncreasePopulation(int i)
	{
		populationCounter ++;
		if (populationCounter == 5)
		{
			Population++;
			ResetPopulationCounter();
			OnPopulationChanged?.Invoke(1);
		}
	}
	
	private void LowerPopulation(int count)
	{
		Population -= count;
		OnPopulationChanged?.Invoke(-count);
	}

	public static void SetStartingPopulation(int startingPopulation)
	{
		Population = startingPopulation;
	}

	public static void ResetPopulationCounter()
	{
		populationCounter = 0;
	}

	protected override void OnUpdate()
	{
		
	}
}
