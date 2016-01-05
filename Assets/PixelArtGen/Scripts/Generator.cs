using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PixelArtGen.Extensions;

namespace PixelArtGen
{
    public class Generator : MonoBehaviour
    {
        static float pixelScale = 4;
        static int rotationNum = 16;
        static float patternScale = 1;
        static float colorNoise = 0.3f;
        static float edgeDarkness = 0.6f;
        public string patternString = " x\no*";
        public bool isMirrorX = false;
        public bool isMirrorY = false;
        public int seed = 0;
        public Color color = Color.red;
        public bool isRotated = true;
        public Texture2D texture;
        int[][] patternPixels;
        int patternPixelsWidth;
        int patternPixelsHeight;

        public Sprite[] Generate()
        {
            GenerateTexture();
            var w = patternPixelsWidth;
            var h = patternPixelsHeight;
            var rotatedPixels = CreateRotated(patternPixels, ref w, ref h, (isRotated ? rotationNum : 1));
            var rotatedPixelsWidth = w;
            var rotatedPixelsHeight = h;
            return rotatedPixels.Select(ps =>
            {
                Random.seed = seed;
                w = rotatedPixelsWidth;
                h = rotatedPixelsHeight;
                var colors = CreateColors(ps, w, h);
                var scaledColors = ScaleColors(colors, ref w, ref h, pixelScale);
                var tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
                tex.filterMode = FilterMode.Point;
                tex.SetPixels(scaledColors);
                tex.Apply();
                return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
            }).ToArray();
        }

        public void OnValidate()
        {
            GenerateTexture();
        }

        public void GenerateTexture()
        {
            if (patternString == null || patternString.Length <= 0)
            {
                return;
            }
            var patterns = patternString.Split(new string[] { "\r\n", "\n", "\r" },
                System.StringSplitOptions.RemoveEmptyEntries);
            Random.seed = seed;
            var pw = patterns.Max(p => p.Length);
            var ph = patterns.Length;
            var w = Mathf.RoundToInt(pw * patternScale);
            var h = Mathf.RoundToInt(ph * patternScale);
            w += (isMirrorX ? 1 : 2);
            h += (isMirrorY ? 1 : 2);
            var pixels = CreatePixels(patterns, pw, ph, w, h, patternScale);
            if (isMirrorX)
            {
                pixels = MirrorX(pixels, ref w, ref h);
            }
            if (isMirrorY)
            {
                pixels = MirrorY(pixels, ref w, ref h);
            }
            patternPixels = CreateEdge(pixels, w, h);
            patternPixelsWidth = w;
            patternPixelsHeight = h;
            var rotatedPixels = CreateRotated(patternPixels, ref w, ref h, 1);
            Random.seed = seed;
            var colors = CreateColors(rotatedPixels[0], w, h);
            var scaledColors = ScaleColors(colors, ref w, ref h, pixelScale);
            texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Point;
            texture.SetPixels(scaledColors);
            texture.Apply();
        }

        int[][] CreatePixels(string[] patterns, int pw, int ph, int w, int h, float scale)
        {
            return h.Range().Select(y =>
            {
                var py = Mathf.FloorToInt((y - 1) / scale);
                return w.Range().Select(x =>
                {
                    var px = Mathf.FloorToInt((x - 1) / scale);
                    if (px < 0 || px >= pw || py < 0 || py >= ph)
                    {
                        return 0;
                    }
                    var c = px < patterns[py].Length ? patterns[py][px] : ' ';
                    var m = 0;
                    if (c == '-')
                    {
                        m = (Random.value < 0.5f ? 1 : 0);
                    }
                    else if (c == 'x' || c == 'X')
                    {
                        m = (Random.value < 0.5f ? 1 : -1);
                    }
                    else if (c == 'o' || c == 'O')
                    {
                        m = -1;
                    }
                    else if (c == '*')
                    {
                        m = 1;
                    }
                    return m;
                }).ToArray();
            }).ToArray();
        }

        int[][] MirrorX(int[][] pixels, ref int w, ref int h)
        {
            w *= 2;
            var _w = w;
            return h.Range().Select(y =>
                _w.Range().Select(x => (x < _w / 2 ? pixels[y][x] : pixels[y][_w - x - 1])).ToArray()
            ).ToArray();
        }

        int[][] MirrorY(int[][] pixels, ref int w, ref int h)
        {
            h *= 2;
            var _w = w;
            var _h = h;
            return h.Range().Select(y =>
                _w.Range().Select(x => (y < _h / 2 ? pixels[y][x] : pixels[_h - y - 1][x])).ToArray()
            ).ToArray();
        }

        int[][] CreateEdge(int[][] pixels, int w, int h)
        {
            return h.Range().Select(y =>
                w.Range().Select(x =>
                ((pixels[y][x] == 0 &&
                        ((x - 1 >= 0 && pixels[y][x - 1] > 0) ||
                            (x + 1 < w && pixels[y][x + 1] > 0) ||
                            (y - 1 >= 0 && pixels[y - 1][x] > 0) ||
                            (y + 1 < h && pixels[y + 1][x] > 0))) ?
                            -1 : pixels[y][x])
                ).ToArray()
            ).ToArray();
        }

        int[][][] CreateRotated(int[][] pixels, ref int w, ref int h, int count)
        {
            var pw = w;
            var ph = h;
            var pcx = w / 2;
            var pcy = h / 2;
            w = Mathf.RoundToInt(w * 1.5f / 2) * 2;
            h = Mathf.RoundToInt(h * 1.5f / 2) * 2;
            var cx = w / 2;
            var cy = h / 2;
            var _w = w;
            var _h = h;
            var offset = new Vector2();
            return count.Range().Select(ai =>
            {
                var angle = (float)ai * 360 / count;
                return _h.Range().Select(y =>
                {
                    return _w.Range().Select(x =>
                    {
                        offset.x = x - cx;
                        offset.y = y - cy;
                        offset = offset.Rotate(angle);
                        var px = Mathf.RoundToInt(offset.x + pcx);
                        var py = Mathf.RoundToInt(offset.y + pcy);
                        if (px < 0 || px >= pw || py < 0 || py >= ph)
                        {
                            return 0;
                        }
                        return pixels[py][px];
                    }).ToArray();
                }).ToArray();
            }).ToArray();
        }

        Color[][] CreateColors(int[][] pixels, int w, int h)
        {
            float hue, saturation, brightness;
            Color.RGBToHSV(color, out hue, out saturation, out brightness);
            return h.Range().Select(y =>
            {
                return w.Range().Select(x =>
                {
                    var p = pixels[h - y - 1][x];
                    if (p != 0)
                    {
                        var b = Mathf.Sin(((float)y / h) * Mathf.PI) * (1 - colorNoise) +
                            Random.value * colorNoise;
                        b = Mathf.Clamp01(b * brightness);
                        if (p == -1)
                        {
                            b *= edgeDarkness;
                        }
                        return Color.HSVToRGB(hue, saturation, b);
                    }
                    else
                    {
                        return Color.clear;
                    }
                }).ToArray();
            }).ToArray();
        }

        Color[] ScaleColors(Color[][] colors, ref int w, ref int h, float scale)
        {
            var pw = w;
            var ph = h;
            w = Mathf.RoundToInt(w * scale);
            h = Mathf.RoundToInt(h * scale);
            var _w = w;
            return h.Range().Aggregate(new List<Color>(), (prev, y) =>
            {
                var py = Mathf.FloorToInt((float)y / scale);
                var cl = _w.Range().Select(x =>
                {
                    var px = Mathf.FloorToInt((float)x / scale);
                    if (px < 0 || px >= pw || py < 0 || py >= ph)
                    {
                        return Color.clear;
                    }
                    return colors[py][px];
                });
                prev.AddRange(cl);
                return prev;
            }).ToArray();
        }
    }
}
