using Unity.Entities;

[DisableAutoCreation]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[UpdateAfter(typeof(TurnSystem))]

partial class PopulationSystem : SystemBase
{
	public static int Population { get; private set; } = 1;

	private static int populationCounter = 0;

	public void OnCreate(ref SystemState state)
	{
	}
	
	protected override void OnStartRunning()
	{
		TurnSystem.OnTurnFinished += IncreasePopulation;
	}
	
	protected override void OnStopRunning()
	{
		TurnSystem.OnTurnFinished -= IncreasePopulation;
	}
	
	private void IncreasePopulation(int i)
	{
		populationCounter ++;
		if (populationCounter == 10)
		{
			Population++;
			ResetPopulationCounter();
		}
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
