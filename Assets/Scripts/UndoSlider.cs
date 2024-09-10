using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UndoSlider : MonoBehaviour
{
    [SerializeField] private Slider _slider;

    IntReactiveProperty CurrentRecordValue = new IntReactiveProperty(0);
    IntReactiveProperty MaxRecordValue = new IntReactiveProperty(0);
    List<Pixels.PixelDatas> _pixelDatas = new List<Pixels.PixelDatas>();
    public IObservable<int> OnValueChangedAsObservable() => _slider.OnValueChangedAsObservable().Select(i => (int)i);

    void Start()
    {
        _slider.minValue = 0;
        _slider.wholeNumbers = true;
        _slider.maxValue = 1;
        CurrentRecordValue.Subscribe(i => _slider.value = i).AddTo(this);
        MaxRecordValue.Subscribe(i => _slider.maxValue = i).AddTo(this);
        _slider.OnValueChangedAsObservable().Subscribe(i => CurrentRecordValue.Value = (int)i).AddTo(this);
    }

    public void AddRecord(Pixels.PixelDatas pixelDatas)
    {
        if (_pixelDatas.Any())
        {
            _pixelDatas.RemoveRange(CurrentRecordValue.Value + 1, _pixelDatas.Count - CurrentRecordValue.Value - 1);
        }

        _pixelDatas.Add(pixelDatas);
        MaxRecordValue.Value = _pixelDatas.Count - 1;
        CurrentRecordValue.Value = _pixelDatas.Count - 1;
    }

    public Pixels.PixelDatas Undo(int index)
    {
        if (index < 0 || index >= _pixelDatas.Count) return null;
        CurrentRecordValue.Value = index;
        return _pixelDatas[index];
    }
}