using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : SingletonBehaviour<GameController>
{
	public static Action<int> OnStageChanged;
	public static Action OnPopulationChanged;
	public static Action OnGameReset;
	public static Action OnGameStarted;
	
	public static bool PlayerInputReceived;
	public static bool IsTurnFinished = true;

	[SerializeField] private Button startGameButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button quitButton;
	[SerializeField] private GameObject startGamePanel;
	[SerializeField] private GameObject gameOverPanel;
	
	[SerializeField] private int startingPopulation;

	public int StartingPopulation => startingPopulation;
	
	public static int CurrentStage { get; private set; } = 1;

	public static PlayerInputSettings PlayerInputSettings { get; private set;}

	private void Awake()
	{
		SystemsController.CreateWorld();
		startGameButton.onClick.AddListener(StartGame);
		restartButton.onClick.AddListener(ResetGame);
		quitButton.onClick.AddListener(QuitGame);
		CompleteStageSystem.OnStageComplete += NextStage;
		GameOverSystem.OnGameOver += GameOver;
		PlayerInputSettings = new PlayerInputSettings();
		PlayerInputSettings.Application.Quit.performed += QuitGame;
	}

	private void StartGame()
	{
		StartCoroutine(SetUpGame());
	}

	private void GameOver()
	{
		gameOverPanel.SetActive(true);
	}

	private void OnDestroy()
	{
		startGameButton.onClick.RemoveAllListeners();
		CompleteStageSystem.OnStageComplete -= NextStage;
		GameOverSystem.OnGameOver -= GameOver;
		PlayerInputSettings.Application.Quit.performed -= QuitGame;
		World.DisposeAllWorlds();
	}

	private IEnumerator SetUpGame()
	{
		SceneManager.LoadSceneAsync("MainScene",LoadSceneMode.Additive);
		yield return new WaitForSeconds(1);
		startGamePanel.SetActive(false);
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
		gameOverPanel.SetActive(false);
	}
}
