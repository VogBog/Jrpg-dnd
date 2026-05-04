using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.TargetChooser
{
    public class TargetChooserViewItem : MonoBehaviour
    {
        [field: SerializeField] public Image Icon { get; private set; }
        [SerializeField] private float _animationDuration;

        public void AnimateOpening()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOScale(Vector3.one, _animationDuration);
        }
    }
}