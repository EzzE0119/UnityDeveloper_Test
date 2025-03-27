using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int _score = 0;
    [SerializeField] private TMP_Text _scoreText;
    GameTimeManager _gameTimeManager;
    [SerializeField] private GameObject _gameFinishedScreen;
    private int Score
    {
        get { return _score; }
        set
        {
            if (_score != value) 
            {
                _score = value;
                OnScoreChanged(); //gets called when Score is changed
            }
        }
    }

    private void Start()
    {
        _gameTimeManager = FindObjectOfType<GameTimeManager>();
    }

    private void OnScoreChanged()
    {
        _scoreText.text = _score + "/5";
        if (_score == 5)
        {
            _gameTimeManager.pauseTimer(true);
            _gameFinishedScreen.SetActive(true);
        }
    }

    public void UpdateScore()
    {
        Score++;
    }
}
