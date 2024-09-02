using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : SingletonBehaviour<GameController>
{
	public static Action<int> OnStageChanged;
	public static Action OnPopulationChanged;
	public static Action OnGameReset;
	public static Action OnGameStarted;
	
	public static bool PlayerInputReceived;
	public static bool IsTurnFinished = true;
	public static PlayerInputSettings PlayerInputSettings { get; private set;}

	public static int CurrentStage { get; private set; } = 1;
	
	public int StartingPopulation => startingPopulation;
	
	[SerializeField] private int startingPopulation;

	private void Awake()
	{
		SystemsController.CreateWorld();
		CompleteStageSystem.OnStageComplete += NextStage;
		PlayerInputSettings = new PlayerInputSettings();
		PlayerInputSettings.Application.Quit.performed += QuitGame;
		UIController.OnStartGamePressed += StartGame;
		UIController.OnResetPressed += ResetGame;
		UIController.OnQuitPressed += QuitGame;
	}

	private void StartGame()
	{
		StartCoroutine(SetUpGame());
	}

	private void OnDestroy()
	{
		CompleteStageSystem.OnStageComplete -= NextStage;
		PlayerInputSettings.Application.Quit.performed -= QuitGame;
		UIController.OnStartGamePressed -= StartGame;
		UIController.OnResetPressed -= ResetGame;
		UIController.OnQuitPressed -= QuitGame;
		World.DisposeAllWorlds();
	}

	private IEnumerator SetUpGame()
	{
		SceneManager.LoadSceneAsync("MainScene",LoadSceneMode.Additive);
		yield return new WaitForSeconds(1);
		SystemsController.SetStartingPopulation();
		SetStage();
		SystemsController.SetSystems();
		OnStageChanged?.Invoke(CurrentStage);
		OnGameStarted?.Invoke();
	}

	private void QuitGame(InputAction.CallbackContext callbackContext)
	{
		QuitGame();
	}
	
	private void QuitGame()
	{
#if UNITY_EDITOR
		
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}

	private static void SetStage()
	{
		CurrentStage = 1;
		SystemsController.SetStage();
	}

	private void NextStage()
	{
		CurrentStage ++;
		ResetStage();
	}

	private void ResetStage()
	{
		SystemsController.ResetStage();
		OnStageChanged?.Invoke(CurrentStage);
		OnPopulationChanged?.Invoke();
	}

	private void ResetGame()
	{
		CurrentStage = 1;
		SystemsController.ResetGame();
		ResetStage();
		OnGameReset?.Invoke();
	}
}
