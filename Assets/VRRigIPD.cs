using UnityEngine;

public class VRRigIPD : MonoBehaviour
{
    public Transform head;      // kamera główna / player head
    public Camera leftEye;
    public Camera rightEye;

    public float ipd = 0.065f;   // default 6.5 cm

    void Update()
    {
        Vector3 basePos = head.position;
        Quaternion rot = head.rotation;

        Vector3 right = head.right;

        leftEye.transform.position = basePos - right * (ipd / 2f);
        rightEye.transform.position = basePos + right * (ipd / 2f);

        leftEye.transform.rotation = rot;
        rightEye.transform.rotation = rot;
    }

    void Start()
    {
        leftEye.enabled = true;
        rightEye.enabled = true;

        leftEye.rect = new Rect(0, 0, 0.5f, 1);
        rightEye.rect = new Rect(0.5f, 0, 0.5f, 1);
    }

    public void SetIPD(float value)
    {
        ipd = value;
    }
}