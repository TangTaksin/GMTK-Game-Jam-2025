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


    private void Start()
    {
        GenerateRoom();
        
    }

    public void GenerateRoom()
    {
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
        bool useCategoryRule = Random.value > 0.5f;

        List<ItemData> mainGroup = null;
        ItemData oddItem = null;

        if (useCategoryRule)
        {
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
            var others = allItems.Where(i => i.category != selectedGroup.Key).ToList();
            if (others.Count == 0)
            {
                Debug.LogError("ไม่พบไอเทมที่แตกต่างใน category");
                return;
            }
            oddItem = others[Random.Range(0, others.Count)];
        }
        else
        {
            // Rule based on weight: เลือกไอเทมเบา 3 ชิ้น + หนัก 1 ชิ้น
            var lightItems = allItems.Where(i => i.weight <= 1f).ToList();
            if (lightItems.Count < 3)
            {
                Debug.LogError("ไม่มีไอเทมเบาเพียงพอ");
                return;
            }
            mainGroup = lightItems.OrderBy(x => Random.value).Take(3).ToList();

            var heavyItems = allItems.Where(i => i.weight > 1f).ToList();
            if (heavyItems.Count == 0)
            {
                Debug.LogError("ไม่มีไอเทมหนักเลย");
                return;
            }
            oddItem = heavyItems[Random.Range(0, heavyItems.Count)];
        }

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
}
