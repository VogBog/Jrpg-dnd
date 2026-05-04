using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Scripts.Effects.VisualEffects
{
    public class SaveThrowEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _mainImage;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private TMP_Text _underlineText;

        [SerializeField] private float _rollingDuration;
        [SerializeField] private float _rollOneNumberStayTime;
        [SerializeField] private float _fadeInDuration;
        [SerializeField] private float _showNumberDuration;
        [SerializeField] private float _fadeOutDuration;

        [SerializeField] private float _sizeByDistanceMultiplier;

        private float _rollingTime;
        private float _oneNumberRollingTime;
        private Transform _camera;

        public async UniTask Animate(
            string underlineText,
            int numberText,
            Color defaultNumberColor,
            Color numberColor,
            Color underlineColor,
            CancellationToken ct)
        {
            _rollingTime = _rollingDuration;
            _underlineText.text = "";
            _mainImage.color = new Color(1, 1, 1, 0);
            _numberText.color = new Color(1, 1, 1, 0);
            _mainImage.gameObject.SetActive(true);
            _mainImage.DOColor(Color.white, _fadeInDuration);
            _numberText.DOColor(defaultNumberColor, _fadeInDuration);

            await UniTask.WaitForSeconds(_rollingDuration, cancellationToken: ct);

            _rollingDuration = 0f;
            _numberText.text = numberText.ToString();
            _numberText.color = numberColor;
            _underlineText.color = underlineColor;
            _underlineText.text = underlineText;
            
            await UniTask.WaitForSeconds(_showNumberDuration, cancellationToken: ct);

            numberColor.a = 0f;
            underlineColor.a = 0f;

            _numberText.DOColor(numberColor, _fadeOutDuration);
            _underlineText.DOColor(underlineColor, _fadeOutDuration);
            _mainImage.DOColor(new Color(1, 1, 1, 0), _fadeOutDuration)
                .OnComplete(() => _mainImage.gameObject.SetActive(false));
        }

        private void Update()
        {
            _camera ??= Camera.main.transform;
            var direction = _camera.position - transform.position;
            transform.LookAt(transform.position - direction);
            var size = direction.magnitude * _sizeByDistanceMultiplier;
            transform.localScale = Vector3.one * size;

            if (_rollingTime > 0f)
            {
                _rollingTime -= Time.deltaTime;
                _oneNumberRollingTime -= Time.deltaTime;

                if (_oneNumberRollingTime < 0f)
                {
                    _numberText.text = Random.Range(1, 21).ToString();
                    _oneNumberRollingTime = _rollOneNumberStayTime;
                }
            }
        }
    }
}