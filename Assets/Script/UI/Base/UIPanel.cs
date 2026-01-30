// Assets/_Project/Scripts/UI/Base/UIPanel.cs

using UnityEngine;
using DG.Tweening;

namespace Game.UI
{
    /// <summary>
    /// Base UI panel with show/hide animations.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float animationDuration = 0.5f;

        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.one * 0.8f;
            _canvasGroup.alpha = 0f;

            _canvasGroup.DOFade(1f, animationDuration);
            transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
        }

        public virtual void Hide()
        {
            _canvasGroup.DOFade(0f, animationDuration);
            transform.DOScale(0.8f, animationDuration).SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
        }
    }
}