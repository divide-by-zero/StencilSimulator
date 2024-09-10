using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    [SerializeField] private StencilStatus[] _stencilStatus;
    [SerializeField] private Pixels _pixels;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Color[] _colors;
    [SerializeField] private Slider _slider;
    [SerializeField] private UndoSlider _undoSlider;

    private CancellationTokenSource _forceCompleteCts;
    private float _delay;

    private void Start()
    {
        _pixels.Initialize(); //初期化

        _undoSlider.OnValueChangedAsObservable()
            .Skip(1)
            .Subscribe(i =>
            {
                _forceCompleteCts?.Cancel();
                _forceCompleteCts = null;
                _pixels.SetPixelDatas(_undoSlider.Undo(i));
            })
            .AddTo(this);

        _undoSlider.AddRecord(_pixels.GetPixelDatas());

        _stencilStatus.Select(status => status.OnDrawButtonClickedAsObservable().Select(_ => status))
            .Merge()
            .Subscribe(async status =>
            {
                _forceCompleteCts?.Cancel();
                _forceCompleteCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                await DrawPixelAsync(status, TimeSpan.FromSeconds(_delay), _forceCompleteCts.Token);
                _undoSlider.AddRecord(_pixels.GetPixelDatas());
            })
            .AddTo(this);

        _clearButton.OnClickAsObservable()
            .Subscribe(async _ =>
            {
                _forceCompleteCts?.Cancel();
                _forceCompleteCts = null;
                await UniTask.Yield(destroyCancellationToken);
                _pixels.SetPixelDatas(_undoSlider.Undo(0));
            })
            .AddTo(this);

        _slider.OnValueChangedAsObservable().Subscribe(f => _delay = f).AddTo(this);

        foreach (var valueTuple in _stencilStatus.Zip(_colors, (status, color) => (status, color)))
        {
            var (status, color) = valueTuple;
            status.SetColor(color);
        }
    }

    public async UniTask DrawPixelAsync(StencilStatus stencilStatus, TimeSpan delay = default, CancellationToken ct = default)
    {
        var rect = stencilStatus.Rect;
        for (var y = (int)rect.y; y < (int)rect.yMax; y++)
        for (var x = (int)rect.x; x < (int)rect.xMax; x++)
        {
            var currentPixel = _pixels.GetPixel(x, y);
            if (currentPixel == null) continue;
            var color = stencilStatus.GetColor();
            using var _ = currentPixel.SetMarker(color);
            var currentId = currentPixel.GetID();
            var newId = stencilStatus.IdRp.Value;
            var max = 7;

            if (delay != default && delay != TimeSpan.Zero)
            {
                await UniTask.Delay(delay, cancellationToken: ct).SuppressCancellationThrow();
            }

            var isSuccess = (CompareFunction)stencilStatus.CompareRp.Value switch
            {
                CompareFunction.Always => true,
                CompareFunction.Equal => currentId == newId,
                CompareFunction.NotEqual => currentId != newId,
                CompareFunction.Less => newId < currentId,
                CompareFunction.LessEqual => newId <= currentId,
                CompareFunction.Greater => newId > currentId,
                CompareFunction.GreaterEqual => newId >= currentId,
                CompareFunction.Disabled => true,
                CompareFunction.Never => false,
            };

            if (isSuccess == false) continue;

            switch ((StencilOp)stencilStatus.OpRp.Value)
            {
                case StencilOp.Keep:
                    currentPixel.SetId(currentId, color);
                    break;
                case StencilOp.Zero:
                    currentPixel.SetId(0, color);
                    break;
                case StencilOp.Replace:
                    currentPixel.SetId(newId, color);
                    break;
                case StencilOp.IncrementSaturate:
                    currentPixel.SetId(Mathf.Min(currentId + 1, max), color);
                    break;
                case StencilOp.IncrementWrap:
                    currentPixel.SetId((currentId + 1) % max, color);
                    break;
                case StencilOp.DecrementSaturate:
                    currentPixel.SetId(Mathf.Max(currentId - 1, 0), color);
                    break;
                case StencilOp.DecrementWrap:
                    currentPixel.SetId((currentId - 1 + max) % max, color);
                    break;
                case StencilOp.Invert:
                    currentPixel.SetId(max - currentId, color);
                    break;
            }
        }
    }
}