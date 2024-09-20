using System;
using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(InputDetectionSystem))]

partial class PopulationSystem : SystemBase
{
	public static Action<int> OnPopulationChanged;
	public static int Population { get; private set; } = 1;
	private static int populationCounter = 0;
	
	protected override void OnStartRunning()
	{
		TurnSystem.OnTurnFinished += IncreasePopulation;
		BotDestroySystem.OnBotDestroyed += LowerPopulation;
	}
	
	protected override void OnStopRunning()
	{
		TurnSystem.OnTurnFinished -= IncreasePopulation;
		BotDestroySystem.OnBotDestroyed -= LowerPopulation;
	}
	
	public static void SetStartingPopulation()
	{
		Population = GameController.Instance.StartingPopulation;
	}

	public static void ResetPopulationCounter()
	{
		populationCounter = 0;
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

	protected override void OnUpdate() { }
}
