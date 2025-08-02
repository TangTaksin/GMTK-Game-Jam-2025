using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    Camera cam;
    AnswerBox answerBox;

    Vector3 cursor_offset;
    Vector3 cursor;

    ItemData itemData;

    private void Awake()
    {
        cam = Camera.main;

        rb2d = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();

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

            var x = Random.Range(-1f, 1f);
            var angle = new Vector2(x, 1);
            var mag = Random.Range(2.5f, 5f);
            var rot = Random.Range(-5f, 5f);

            rb2d.AddForce(angle * mag, ForceMode2D.Impulse);
            rb2d.AddTorque(rot, ForceMode2D.Impulse);
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
