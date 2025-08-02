using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomGenerator : MonoBehaviour
{
 [Header("ไอเทมทั้งหมดในเกม")]
    public List<ItemData> allItems;

    [Header("ตำแหน่งวางไอเทม 4 จุด")]
    public Transform[] spawnPoints;  // ต้องมี 4 จุด

    [Header("Prefab ไอเทม")]
    public GameObject itemPrefab;

    private ItemData correctAnswer;

    public delegate void RoomEvent(bool _bool);
    public static RoomEvent RoomGeneratedEvent;
    public static RoomEvent AnswerEvent;

    private void OnEnable()
    {
        GameManager.StartEvent += GenerateRoom;
        GameManager.NextRoomEvent += GenerateRoom;
        AnswerBox.AddEvent += checkAnswer;
    }

    private void OnDisable()
    {
        GameManager.StartEvent -= GenerateRoom;
        GameManager.NextRoomEvent -= GenerateRoom;
        AnswerBox.AddEvent -= checkAnswer;
    }

    public void GenerateRoom()
    {
        print("generating");

        if (spawnPoints.Length < 4)
        {
            Debug.LogError("ต้องมี spawnPoints อย่างน้อย 4 จุด");
            return;
        }

        // ล้างไอเทมเก่าในห้อง
        foreach (var point in spawnPoints)
        {
            foreach (Transform child in point)
                Destroy(child.gameObject);
        }

        // สุ่ม rule ง่าย ๆ สองแบบ
        //bool useCategoryRule = Random.value > 0;

        List<ItemData> mainGroup = null;
        ItemData oddItem = null;

        // หากลุ่มไอเทมที่มี category เดียวกันอย่างน้อย 3 ชิ้น
        var groups = allItems.GroupBy(i => i.category)
                                .Where(g => g.Count() >= 3)
                                .ToList();

        if (groups.Count == 0)
        {
            Debug.LogError("ไม่มีกลุ่ม category ที่มีไอเทม >=3 ชิ้น");
            return;
        }

        var selectedGroup = groups[Random.Range(0, groups.Count)];
        mainGroup = selectedGroup.OrderBy(x => Random.value).Take(3).ToList();

        // หาไอเทมที่ category แตกต่าง (odd one out)
        var oddGroup = allItems.Where(i => i.category != selectedGroup.Key).ToList();

        if (oddGroup.Count == 0)
        {
            Debug.LogError("ไม่พบไอเทมที่แตกต่างใน category");
            return;
        }

        oddItem = oddGroup[Random.Range(0, oddGroup.Count)];

        RoomGeneratedEvent?.Invoke(true);

        // รวมไอเทม 4 ชิ้น แล้วสลับตำแหน่งสุ่ม
        List<ItemData> itemsToSpawn = new List<ItemData>(mainGroup) { oddItem };
        itemsToSpawn = itemsToSpawn.OrderBy(x => Random.value).ToList();

        // สร้างในตำแหน่ง spawnPoints
        for (int i = 0; i < 4; i++)
        {
            GameObject obj = Instantiate(itemPrefab, spawnPoints[i].position, Quaternion.identity, spawnPoints[i]);
            var display = obj.GetComponent<ItemDisplay>();
            if (display != null)
                display.Setup(itemsToSpawn[i]);
        }

        correctAnswer = oddItem;
        Debug.Log("คำตอบ: " + correctAnswer.itemName + " (แตกต่าง)");
    }

    public void checkAnswer(AnswerBox box)
    {
        var answer = box.ReadObjects().GetData();

        bool isCorrect = false;

        if (answer == correctAnswer)
        {
            isCorrect = true;
        }

        AnswerEvent?.Invoke(isCorrect);
    }
}
