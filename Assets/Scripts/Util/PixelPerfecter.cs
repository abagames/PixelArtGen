using UnityEngine;

public class PixelPerfecter : MonoBehaviour
{
    public float pixelsPerUnit = 100;

    void Awake()
    {
        var cam = GetComponent<Camera>();
        cam.orthographicSize = Screen.height / pixelsPerUnit / 2;
    }
}
