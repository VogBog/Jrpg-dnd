using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Helpers.DOTweenExtensions;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Effects.VisualEffects
{
    public class TextPopupEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _yOffset;
        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _fadeOutDuration;
        [SerializeField] private float _forceAmplitude;
        [SerializeField] private float _sizeByDistanceMultiplier;
        
        private Transform _camera;

        public UniTask PopupAsync(string text, Color color, CancellationToken ct)
        {
            return PopupAsyncInternal(text, color, ct, async _ =>
            {
                var pos = transform.position;
                pos.y += _yOffset;
                transform.DOMove(pos, _animationDuration);
            });
        }

        public UniTask PopupForced(string text, Color color, CancellationToken ct)
        {
            return PopupAsyncInternal(text, color, ct, async _ =>
            {
                var xzPos = transform.position;
                xzPos += new Vector3(
                    Random.Range(-_forceAmplitude, _forceAmplitude), 
                    0, 
                    Random.Range(-_forceAmplitude, _forceAmplitude));

                transform.DOMoveX(xzPos.x, _animationDuration / 2f);
                transform.DOMoveZ(xzPos.z, _animationDuration / 2f);

                float y = xzPos.y + _forceAmplitude * Random.Range(0.5f, 1f);
                await transform.DOMoveY(y, _animationDuration / 2f)
                    .SetEase(Ease.InQuad)
                    .ToUniTask(ct);
            });
        }

        private async UniTask PopupAsyncInternal(string text, Color color, CancellationToken ct,
            Func<CancellationToken, UniTask> moveTask)
        {
            color.a = 0f;
            
            _text.text = text;
            _text.color = color;
            gameObject.SetActive(true);

            color.a = 1f;

            _text.DOColor(color, _fadeInDuration);
            moveTask.Invoke(ct).Forget();
            await UniTask.WaitForSeconds(_animationDuration - _fadeOutDuration, cancellationToken: ct);

            color.a = 0f;
            await _text.DOColor(color, _fadeOutDuration).ToUniTask(ct);
            
            gameObject.SetActive(false);
        }

        private void Update()
        {
            _camera ??= Camera.main.transform;
            var direction = _camera.position - transform.position;
            transform.LookAt(transform.position - direction);
            var size = direction.magnitude * _sizeByDistanceMultiplier;
            transform.localScale = Vector3.one * size;
        }
    }
}