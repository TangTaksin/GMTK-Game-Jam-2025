using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            Debug.LogWarning("Rigidbody2D ไม่พบใน ItemDisplay นี้");
        }
    }

    public void Setup(ItemData item)
    {
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
        }
    }
}
