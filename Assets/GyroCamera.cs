using UnityEngine;

public class GyroCamera : MonoBehaviour
{
    private Gyroscope gyro;
    private bool gyroEnabled;

    private Quaternion baseRotation;
    private Quaternion calibration;

    public float smoothSpeed = 0f;

    void Start()
    {
        gyroEnabled = EnableGyro();

        // korekta układu Unity
        baseRotation = Quaternion.Euler(90f, 0f, 0f);

        calibration = Quaternion.identity;
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            return true;
        }
        return false;
    }

    Quaternion GetGyroRotation()
    {
        Quaternion q = gyro.attitude;

        // 🔥 KLUCZOWA KONWERSJA (odwraca osie poprawnie)
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    void Update()
    {
        if (!gyroEnabled) return;

        Quaternion raw = baseRotation * GetGyroRotation();

        // 🔘 RECENTER (tap ekran)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            calibration = Quaternion.Inverse(raw);
        }

        Quaternion target = calibration * raw;

        if (smoothSpeed <= 0f)
        {
            transform.localRotation = target;
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                target,
                Time.deltaTime * smoothSpeed
            );
        }
    }
}