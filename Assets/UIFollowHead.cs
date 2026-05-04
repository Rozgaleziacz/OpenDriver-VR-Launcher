using UnityEngine;

public class UIFollowHead : MonoBehaviour
{
    public Transform head;
    public Vector3 offset = new Vector3(0, 0, 2f);
    public float smooth = 5f;

    void Update()
    {
        Vector3 targetPos = head.position + head.forward * offset.z;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * smooth
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(transform.position - head.position),
            Time.deltaTime * smooth
        );
    }
}