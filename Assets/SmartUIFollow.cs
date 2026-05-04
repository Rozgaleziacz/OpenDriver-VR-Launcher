using UnityEngine;

public class SmartUIFollow : MonoBehaviour
{
    public Transform head;

    public float distance = 2f;
    public float followSpeed = 5f;

    public float angleThreshold = 30f; // ile stopni zanim zacznie się ruszać

    void Update()
    {
        Vector3 headForward = head.forward;
        headForward.y = 0;
        headForward.Normalize();

        Vector3 directionToUI = (transform.position - head.position).normalized;
        directionToUI.y = 0;

        float angle = Vector3.Angle(headForward, directionToUI);

        // 👉 tylko jeśli przekroczysz próg
        if (angle > angleThreshold)
        {
            Vector3 targetPos = head.position + headForward * distance;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSpeed
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(transform.position - head.position),
                Time.deltaTime * followSpeed
            );
        }
    }
}