using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : SingletonBehaviour<GameController>
{
	public static Action<int> OnStageChanged;
	public static Action OnPopulationChanged;
	
	public static Action OnEntitySceneLoaded;
	public static Action OnGameStarted;
	public static Action OnGameReset;
	
	public static bool PlayerInputReceived;
	public static bool IsTurnFinished = true;
	public static PlayerInputSettings PlayerInputSettings { get; private set;}

	public static int CurrentStage { get; private set; } = 1;
	
	public int StartingPopulation => startingPopulation;
	
	[SerializeField] private int startingPopulation;

	private void Awake()
	{
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
	}

	private IEnumerator SetUpGame()
	{
		AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("MainScene",LoadSceneMode.Additive);

		while (!asyncLoadLevel.isDone)
			yield return 0;

		yield return new WaitForSeconds(1);
		
		CurrentStage = 1;
		OnEntitySceneLoaded?.Invoke();
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

	private void NextStage()
	{
		CurrentStage ++;
		ResetStage();
	}

	private void ResetStage()
	{
		OnStageChanged?.Invoke(CurrentStage);
		OnPopulationChanged?.Invoke();
	}

	private void ResetGame()
	{
		CurrentStage = 1;
		ResetStage();
		OnGameReset?.Invoke();
	}
}
