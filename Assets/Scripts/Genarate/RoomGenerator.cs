using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    #region Fields

    [Header("ไอเทมทั้งหมดในเกม")]
    public List<ItemData> allItems;

    [Header("ตำแหน่งวางไอเทม 4 จุด")]
    public Transform[] spawnPoints;

    [Header("Prefab ไอเทม")]
    public GameObject itemPrefab;

    private ItemData correctAnswer;

    private enum QuizType
    {
        Category,
        Special
    }

    #endregion

    #region Events

    public delegate void RoomEvent(bool success);
    public static RoomEvent RoomGeneratedEvent;
    public static RoomEvent AnswerEvent;

    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        GameManager.StartEvent += GenerateRoom;
        GameManager.NextRoomEvent += GenerateRoom;
        AnswerBox.AddEvent += CheckAnswer;
    }

    private void OnDisable()
    {
        GameManager.StartEvent -= GenerateRoom;
        GameManager.NextRoomEvent -= GenerateRoom;
        AnswerBox.AddEvent -= CheckAnswer;
    }

    #endregion

    #region Core Logic

    public void GenerateRoom()
    {
        Debug.Log("Generating Room...");

        if (spawnPoints.Length < 4)
        {
            Debug.LogError("ต้องมี spawnPoints อย่างน้อย 4 จุด");
            return;
        }

        ClearPreviousItems();

        QuizType quizType = (QuizType)Random.Range(0, 2);

        switch (quizType)
        {
            case QuizType.Category:
                GenerateCategoryQuiz();
                break;
            case QuizType.Special:
                GenerateSpecialQuiz();
                break;
        }
    }

    private void ClearPreviousItems()
    {
        foreach (var point in spawnPoints)
        {
            foreach (Transform child in point)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void GenerateCategoryQuiz()
    {
        var validGroups = allItems
            .GroupBy(i => i.category)
            .Where(g => g.Count() >= 3)
            .ToList();

        if (validGroups.Count == 0)
        {
            Debug.LogWarning("ไม่มีกลุ่ม category ที่มีไอเทม >= 3 ชิ้น");
            return;
        }

        var selectedGroup = validGroups[Random.Range(0, validGroups.Count)];
        var mainGroup = selectedGroup.OrderBy(_ => Random.value).Take(3).ToList();
        var oddCandidates = allItems.Where(i => i.category != selectedGroup.Key).ToList();

        if (oddCandidates.Count == 0)
        {
            Debug.LogWarning("ไม่พบไอเทมที่แตกต่างใน category");
            return;
        }

        var oddItem = oddCandidates[Random.Range(0, oddCandidates.Count)];
        CreateQuiz(mainGroup, oddItem, $"category: {selectedGroup.Key}");
    }

    private void GenerateSpecialQuiz()
    {
        var specials = allItems.Where(i => i.special).ToList();
        var nonSpecials = allItems.Where(i => !i.special).ToList();

        if (specials.Count >= 3 && nonSpecials.Count >= 1)
        {
            var mainGroup = specials.OrderBy(_ => Random.value).Take(3).ToList();
            var oddItem = nonSpecials[Random.Range(0, nonSpecials.Count)];
            CreateQuiz(mainGroup, oddItem, "special: TRUE");
        }
        else if (nonSpecials.Count >= 3 && specials.Count >= 1)
        {
            var mainGroup = nonSpecials.OrderBy(_ => Random.value).Take(3).ToList();
            var oddItem = specials[Random.Range(0, specials.Count)];
            CreateQuiz(mainGroup, oddItem, "special: FALSE");
        }
        else
        {
            Debug.LogWarning("ไม่สามารถสร้างคำถามแบบ special ได้ — fallback ไป category");
            GenerateCategoryQuiz();
        }
    }

    private void CreateQuiz(List<ItemData> groupItems, ItemData oddItem, string debugInfo)
    {
        correctAnswer = oddItem;
        RoomGeneratedEvent?.Invoke(true);

        var itemsToSpawn = new List<ItemData>(groupItems) { oddItem };
        itemsToSpawn = itemsToSpawn.OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < 4; i++)
        {
            var spawnPoint = spawnPoints[i];
            var obj = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);

            var display = obj.GetComponent<ItemDisplay>();
            if (display != null)
            {
                display.Setup(itemsToSpawn[i]);
            }
        }

        Debug.Log($"คำตอบ: {correctAnswer.itemName} (แตกต่าง - {debugInfo})");
    }

    public void CheckAnswer(AnswerBox box)
    {
        var submittedItem = box.ReadObjects().GetData();
        bool isCorrect = submittedItem == correctAnswer || submittedItem.Equals(correctAnswer);
        AnswerEvent?.Invoke(isCorrect);
    }

    #endregion
}
