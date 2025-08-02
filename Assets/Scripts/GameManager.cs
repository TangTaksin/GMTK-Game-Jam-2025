using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    int roomTraversed = 0;
    public int roomTraverseNeed;
    [Space]
    public TextMeshProUGUI roomText;

    public delegate void GameEvent();
    public static GameEvent StartEvent;
    public static GameEvent NextRoomEvent;

    public string gameClearScene;

    private void OnEnable()
    {        
        RoomGenerator.AnswerEvent += NextRoom;

    }

    private void OnDisable()
    {
        RoomGenerator.AnswerEvent -= NextRoom;
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

    public void NextRoom(bool isCorrect)
    {
        

        if (isCorrect)
        {
            roomTraversed++;
        }
        else
        {
            roomTraversed = 0;
        }

        if (roomTraversed >= roomTraverseNeed)
        {
            SceneManager.LoadScene(gameClearScene);
        }

        NextRoomEvent?.Invoke(); 
        UpdateText();
    }

    private void UpdateText()
    {
        roomText.text = roomTraversed.ToString();
    }
}
