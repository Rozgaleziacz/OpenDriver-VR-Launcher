using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ColorTracker : MonoBehaviour
{
    [Header("UI")]
    public RawImage cameraView;

    [Header("Tracked Object")]
    public Transform trackedObject;

    [Header("Camera")]
    public int camWidth = 320;
    public int camHeight = 240;

    [Header("Detection")]
    public float hueRange = 0.05f;
    public float minSaturation = 0.6f;
    public float minValue = 0.6f;

    [Header("Performance")]
    public int pixelSkip = 6;

    [Header("Movement")]
    public float smoothSpeed = 15f;
    public float rangeX = 2f;
    public float rangeY = 2f;
    public float depthMultiplier = 5f;

    private WebCamTexture cam;
    private float targetHue = 0f;
    private bool hasColor = false;

    // 📁 plik zapisu
    private string SavePath;

    [System.Serializable]
    class SaveData
    {
        public float hue;
        public bool hasColor;
    }

    void Start()
    {
        SavePath = Path.Combine(Application.persistentDataPath, "color_save.json");

        LoadColor();

        cam = new WebCamTexture(camWidth, camHeight);
        cam.Play();

        if (cameraView != null)
            cameraView.texture = cam;
    }

    void Update()
    {
        if (cam == null || !cam.isPlaying || cam.width < 100)
            return;

        HandlePick();
        Track();
    }

    // 🎯 klik = wybór koloru
    void HandlePick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Input.mousePosition;

            int px = (int)(pos.x / Screen.width * cam.width);
            int py = (int)(pos.y / Screen.height * cam.height);

            Color c = cam.GetPixel(px, py);

            Color.RGBToHSV(c, out targetHue, out float s, out float v);

            hasColor = true;

            SaveColor();

            Debug.Log("🎯 Saved hue: " + targetHue);
        }
    }

    // 🧠 tracking
    void Track()
    {
        if (!hasColor) return;

        Color[] pixels = cam.GetPixels();

        float sumX = 0;
        float sumY = 0;
        int count = 0;

        for (int y = 0; y < cam.height; y += pixelSkip)
        {
            for (int x = 0; x < cam.width; x += pixelSkip)
            {
                Color c = pixels[y * cam.width + x];

                Color.RGBToHSV(c, out float h, out float s, out float v);

                float diff = Mathf.Abs(h - targetHue);
                diff = Mathf.Min(diff, 1f - diff);

                bool match =
                    diff < hueRange &&
                    s > minSaturation &&
                    v > minValue;

                if (match)
                {
                    sumX += x;
                    sumY += y;
                    count++;
                }
            }
        }

        if (count > 30)
        {
            float avgX = sumX / count;
            float avgY = sumY / count;

            float normX = avgX / cam.width;
            float normY = avgY / cam.height;

            float x = (normX - 0.5f) * 2f;
            float y = (normY - 0.5f) * 2f;
            y = -y;

            // 📏 GŁĘBIA: im więcej pikseli, tym bliżej
            float depth = Mathf.Clamp(count / 200f, 0.5f, 2f);

            Vector3 targetPos =
                Camera.main.transform.position +
                Camera.main.transform.forward * (depth * depthMultiplier) +
                Camera.main.transform.right * x * rangeX +
                Camera.main.transform.up * y * rangeY;

            trackedObject.position = Vector3.Lerp(
                trackedObject.position,
                targetPos,
                Time.deltaTime * smoothSpeed
            );
        }
    }

    // 💾 zapis do pliku
    void SaveColor()
    {
        SaveData data = new SaveData
        {
            hue = targetHue,
            hasColor = hasColor
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SavePath, json);
    }

    // 📥 odczyt z pliku
    void LoadColor()
    {
        if (!File.Exists(SavePath))
            return;

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        targetHue = data.hue;
        hasColor = data.hasColor;

        Debug.Log("💾 Loaded hue from file: " + targetHue);
    }
}