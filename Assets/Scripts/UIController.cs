using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : SingletonBehaviour<UIController>
{
    public static Action OnStartGamePressed;
    public static Action OnResetPressed;
    public static Action OnQuitPressed;
    
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private GameObject gameOverPanel;
    
    void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(ResetGame);
        quitButton.onClick.AddListener(QuitGame);
        GameOverSystem.OnGameOver += GameOver;
    }

    private void OnDestroy()
    {
        startGameButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        GameOverSystem.OnGameOver -= GameOver;
    }
    
    
    private void StartGame()
    {
        startGamePanel.SetActive(false);
        OnStartGamePressed?.Invoke();
    }
    
    private void ResetGame()
    {
        gameOverPanel.SetActive(false);
        OnResetPressed?.Invoke();
    }
    
    private void QuitGame()
    {
        OnQuitPressed?.Invoke();
    }
    
    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
