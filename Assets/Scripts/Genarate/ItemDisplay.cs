using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;
    PolygonCollider2D polygon2d;

    Camera cam;
    AnswerBox answerBox;

    Vector3 cursor_offset;
    Vector3 cursor;

    ItemData itemData;

    private readonly List<Vector2> m_PhysicsShapePath = new();

    private void Awake()
    {
        cam = Camera.main;

        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb2d == null)
        {
            Debug.LogWarning("Rigidbody2D ไม่พบใน ItemDisplay นี้");
        }
    }

    public void Setup(ItemData item)
    {
        itemData = item;

        if (spriteRenderer != null)
            spriteRenderer.sprite = item.icon;

        polygon2d = gameObject.AddComponent<PolygonCollider2D>();

        gameObject.name = item.itemName;

        if (rb2d != null)
        {
            // ตั้งค่า mass ตาม weight
            rb2d.mass = item.weight;

            // ตั้งค่า angularDamping แปรผกผันกับ weight
            // น้ำหนักมาก → หมุนน้อย
            float minDamp = 0.1f;
            float maxDamp = 2.0f;
            float normalizedWeight = Mathf.InverseLerp(0.1f, 3f, item.weight); // ปรับช่วงตามไอเทมจริง
            rb2d.angularDamping = Mathf.Lerp(maxDamp, minDamp, normalizedWeight);

            // (ถ้าต้องการปรับ linear drag ด้วย ก็ใช้ rb2d.drag = ... ได้)

            // สุ่มเล็กน้อยทางซ้าย/ขวา แต่เน้นขึ้นเป็นหลัก
            Vector2 direction = new Vector2(Random.Range(-1f, 1f), 1f).normalized;

            // สุ่มแรงที่ใช้พุ่งขึ้น
            float forceMagnitude = Random.Range(4f, 8f); // ปรับให้แรงขึ้นกว่าเดิม

            // สุ่มทอร์กเพื่อให้หมุน
            float torque = Random.Range(-10f, 10f); // เพิ่ม torque ถ้าต้องการหมุนแรงขึ้น

            // เพิ่มแรงดันในทิศทางขึ้น
            rb2d.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);

            // เพิ่มแรงหมุน
            rb2d.AddTorque(torque, ForceMode2D.Impulse);


        }
    }

    public ItemData GetData()
    {
        return itemData;
    }


    private void OnMouseDown()
    {
        cursor = cam.ScreenToWorldPoint(Input.mousePosition);
        cursor_offset = transform.position - cursor;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.pickUp_SFX);
    }

    private void OnMouseDrag()
    {
        cursor = cam.ScreenToWorldPoint(Input.mousePosition);

        rb2d.MovePosition(cursor_offset + cursor);
        rb2d.linearVelocity = Input.mousePositionDelta;
        rb2d.angularVelocity = 0;
    }



    private void Update()
    {

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        var detected = collision.gameObject;

        detected.TryGetComponent<AnswerBox>(out var abox);
        answerBox = abox;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var detected = collision.gameObject;

        if (detected == answerBox.gameObject)
            answerBox = null;
    }

    private void OnMouseUp()
    {
        if (answerBox)
        {
            answerBox.Add(this);

            gameObject.SetActive(false);
        }
    }

}
