using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public TextMeshProUGUI gameTimerText;
    public float totalGameTime = 120f;
    bool timerPaused = false;

    [SerializeField] private GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGameTimer());
    }

    private IEnumerator StartGameTimer()
    {
        float time = totalGameTime;
        if (!timerPaused)
        {
            while (time >= 0)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);
                gameTimerText.text = $"{minutes:00}:{seconds:00}";
                yield return null;
                time -= Time.deltaTime;
            }

            gameTimerText.text = "00:00";
            gameOverScreen.SetActive(true);
        }
    }

    public void pauseTimer(bool a_pause)
    {
        timerPaused = a_pause;
    }
}
