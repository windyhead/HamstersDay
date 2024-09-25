using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private GameOverScreen gameOverPanel;
    
    void Start()
    {
        GameController.OnGameStarted += HideGuidePanel;
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
        GameController.OnGameStarted -= HideGuidePanel;
    }
    
    private void StartGame()
    {
        startGamePanel.SetActive(false);
        OnStartGamePressed?.Invoke();
    }

    private void HideGuidePanel()
    {
        StartCoroutine(WaitAndHideGuide());
    }

    private IEnumerator WaitAndHideGuide()
    {
        yield return new WaitForSeconds(2);
        guidePanel.SetActive(false);
    }

    private void ResetGame()
    {
        gameOverPanel.gameObject.SetActive(false);
        OnResetPressed?.Invoke();
    }
    
    private void QuitGame()
    {
        OnQuitPressed?.Invoke();
    }
    
    private void GameOver()
    {
        gameOverPanel.Show();
        gameOverPanel.gameObject.SetActive(true);
    }
}
