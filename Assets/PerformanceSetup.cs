using UnityEngine;

public class PerformanceSetup : MonoBehaviour
{
    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        // 🔥 rozdzielczość VR
        Screen.SetResolution(1920, 1080, true);

        // FPS limit
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }
}