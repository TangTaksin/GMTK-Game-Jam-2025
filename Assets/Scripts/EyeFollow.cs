using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    [Header("Eye Movement")]
    public Transform pupil;
    public float maxDistance = 0.1f;
    public float followSpeed = 10f;


    [Header("Jitter")]
    public bool enableJitter = false;
    public float jitterStrength = 0.005f;

    private Vector3 initialLocalPos;

    private void Start()
    {
        if (pupil != null)
            initialLocalPos = pupil.localPosition;
    }

    private void Update()
    {
        // มองตามเมาส์
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 dir = (mouseWorldPos - transform.position).normalized;
        Vector3 targetLocalPos = initialLocalPos + dir * maxDistance;

        // เพิ่ม Jitter
        if (enableJitter)
        {
            targetLocalPos += (Vector3)Random.insideUnitCircle * jitterStrength;
        }

        // ขยับอย่างนุ่มนวล
        pupil.localPosition = Vector3.Lerp(pupil.localPosition, targetLocalPos, followSpeed * Time.deltaTime);
    }
}
