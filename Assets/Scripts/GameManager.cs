using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    int roomTraversed = 0;
    public int roomTraversedNeeded;
    [Space]
    public TextMeshProUGUI roomText;

    public delegate void GameEvent();
    public static GameEvent StartEvent;
    public static GameEvent NextRoomEvent;

    public string gameClearScene;

    private void OnEnable()
    {        
        RoomGenerator.AnswerEvent += TransitionToNextRoom;

    }

    private void OnDisable()
    {
        RoomGenerator.AnswerEvent -= TransitionToNextRoom;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        roomTraversed = 0;
        StartEvent?.Invoke();

        UpdateText();
    }

    bool corrected;

    void TransitionToNextRoom(bool isCorrect)
    {
        Transition.CalledFadeIn?.Invoke();
        Transition.FadeInOver += NextRoom;

        corrected = isCorrect;
    }

    public void NextRoom()
    {
        Transition.FadeInOver -= NextRoom;

        if (corrected)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.correct_answer_SFX);
            roomTraversed++;
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.wrong_answer_SFX);
            roomTraversed = 0;
        }

        if (roomTraversed >= roomTraversedNeeded)
        {
            SceneManager.LoadScene(gameClearScene);
            return;
        }

        NextRoomEvent?.Invoke(); 
        UpdateText();
    }

    private void UpdateText()
    {
        if (roomText)
        roomText.text = roomTraversed.ToString();
    }
}
