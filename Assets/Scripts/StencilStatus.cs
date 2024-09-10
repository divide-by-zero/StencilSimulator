using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class StencilStatus : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _comparionDropdown;
    [SerializeField] private TMP_Dropdown _idDropdown;
    [SerializeField] private TMP_Dropdown _operationDropdown;
    [SerializeField] private Button _drawButton;
    [SerializeField] private Rect _rect;
    [SerializeField] private Image _image;

    public ReadOnlyReactiveProperty<int> CompareRp { private set; get; }
    public ReadOnlyReactiveProperty<int> IdRp { private set; get; }
    public ReadOnlyReactiveProperty<int> OpRp { private set; get; }

    public IObservable<Unit> OnDrawButtonClickedAsObservable()
    {
        return _drawButton.OnClickAsObservable();
    }

    public Rect Rect => _rect;

    private void Start()
    {
        CompareRp = _comparionDropdown.onValueChanged.AsObservable().ToReadOnlyReactiveProperty();
        IdRp = _idDropdown.onValueChanged.AsObservable().ToReadOnlyReactiveProperty();
        OpRp = _operationDropdown.onValueChanged.AsObservable().ToReadOnlyReactiveProperty();
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }

    public Color GetColor() => _image.color;
}