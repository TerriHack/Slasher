using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float gameDuration;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int scoreToReach;
  
    private bool phaseZeroEnded;
    
    private bool _timerStarted;
    private float _timer;

    private int _currentScore;
    
    public Action startFleeingPhase;

    private GameObject player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        player = GameObject.FindGameObjectWithTag("Player"); //DEBUG
        
        StartGameTimer();
    }
    private void Update()
    {
        GameTimer();
    }

    public void FirstBlood()
    {
        if (phaseZeroEnded) return;
        
        Debug.Log("FIIIIIIRST BLOOOOOOOOD");
        
        phaseZeroEnded = true;
        startFleeingPhase?.Invoke();
    }
    public GameObject GetPlayer()
    {
        return player;
    }

    private void StartGameTimer()
    {
        _timerStarted = true;
        _timer = gameDuration;
    }
    private void GameTimer()
    {
        if(!_timerStarted) return;
        _timer -= Time.deltaTime;
        
        timerText.text = FormatTime(_timer);

        if (_timer <= 0)
        {
            ReturnToMainMenu();
        }
    }
    private string FormatTime(float time)
    {
        int minute = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{00:00}:{1:00}", minute, seconds);
    }
    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    

    public void UpdateScore(int scoreToAdd)
    {
        _currentScore += scoreToAdd;
        scoreText.text = _currentScore.ToString();

        if (_currentScore >= scoreToReach)
        {
            Application.Quit();
        }
    }
}
