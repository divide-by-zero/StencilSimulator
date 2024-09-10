using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Pixel : MonoBehaviour
{
    [SerializeField] private TMP_Text _tmpText;
    [SerializeField] private Image _image;
    [SerializeField] private Image _currentTargetMarker;

    private int _id;

    public void SetId(int id, Color color)
    {
        _id = id;
        _tmpText.text = id.ToString();
        _image.color = color;
    }

    public Color GetColor() => _image.color;
    public int GetID() => _id;

    public IDisposable SetMarker(Color color)
    {
        _currentTargetMarker.gameObject.SetActive(true);
        return Disposable.Create(() => _currentTargetMarker.gameObject.SetActive(false));
    }
}