using UnityEngine;

public class Pixels : MonoBehaviour
{
    [SerializeField] private Pixel _pixelPrefab;

    private Pixel[,] _pixels = new Pixel[16, 16];

    public void Initialize()
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                _pixels[y, x] = Instantiate(_pixelPrefab, Vector3.zero, Quaternion.identity, transform);
            }
        }

        ClearPixels(0, Color.white);
    }

    public void PutPixel(int x, int y, int id, Color color)
    {
        if (x < 0 || x >= 16 || y < 0 || y >= 16) return;
        _pixels[x, y].SetId(id, color);
    }

    public Pixel GetPixel(int x, int y)
    {
        if (x < 0 || x >= 16 || y < 0 || y >= 16) return null;
        return _pixels[x, y];
    }

    public void ClearPixels(int id, Color color)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                _pixels[x, y].SetId(id, color);
            }
        }
    }

    public PixelDatas GetPixelDatas()
    {
        var pixelDatas = new PixelDatas();
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                var pixel = _pixels[x, y];
                var pixelData = new PixelData
                {
                    id = pixel.GetID(),
                    color = pixel.GetColor()
                };
                pixelDatas.Value[x, y] = pixelData;
            }
        }

        return pixelDatas;
    }

    public void SetPixelDatas(PixelDatas pixelDatas)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                var pixelData = pixelDatas.Value[x, y];
                PutPixel(x, y, pixelData.id, pixelData.color);
            }
        }
    }

    public class PixelData
    {
        public int id;
        public Color color;
    }

    public class PixelDatas
    {
        public PixelData[,] Value = new PixelData[16, 16];
    }
}