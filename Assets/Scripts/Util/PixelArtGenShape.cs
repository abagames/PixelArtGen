using UnityEngine;

public class PixelArtGenShape : MonoBehaviour
{
    public static float pixelScale = 4;
    public static float pixelsPerUnit = 100;
    public static int rotationNum = 16;
    float pixelRatio;
    float angleInterval;
    GameObject target;
    Sprite[] rotatedSprites;

    public void Init(GameObject target, Sprite[] rotatedSprites = null)
    {
        this.target = target;
        this.rotatedSprites = rotatedSprites;
    }

    void Start()
    {
        pixelRatio = pixelScale / pixelsPerUnit;
        angleInterval = 360.0f / rotationNum;
        var t = GetComponent<Transform>();
        t.position = new Vector3(float.MinValue * 0.1f, float.MinValue * 0.1f);
    }

    void Update()
    {
        var targetTransform = target.GetComponent<Transform>();
        var position = targetTransform.position;
        var angle = targetTransform.eulerAngles.z;
        var t = GetComponent<Transform>();
        t.position = new Vector3
            (Mathf.Floor(position.x / pixelRatio) * pixelRatio,
            Mathf.Floor(position.y / pixelRatio) * pixelRatio);
        if (rotatedSprites != null)
        {
            var ai = (int)Util.Wrap(Mathf.Round(angle / angleInterval), 0, 16);
            GetComponent<SpriteRenderer>().sprite = rotatedSprites[ai];
        }
        else
        {
            t.rotation = Quaternion.Euler(0, 0, Mathf.Round(angle / angleInterval) * angleInterval);
        }
    }

    static public Vector2 AlignPosition(Vector2 position)
    {
        var pixelRatio = pixelScale / pixelsPerUnit;
        position.x = Mathf.Floor(position.x / pixelRatio) * pixelRatio;
        position.y = Mathf.Floor(position.y / pixelRatio) * pixelRatio;
        return position;
    }
}
