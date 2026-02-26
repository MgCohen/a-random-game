using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace CardMatch.PlayView
{
    [RequireComponent(typeof(RectTransform))]
    public class CardAnimator : MonoBehaviour
    {
        [Header("Flip")]
        [SerializeField] private float flipDuration = 0.2f;
        [SerializeField] private Ease flipEase = Ease.OutQuad;
        [SerializeField] private GameObject backObject;

        [Header("Shake")]
        [SerializeField] private float shakeDuration = 0.3f;
        [SerializeField] private float shakeStrength = 20f;
        [SerializeField] private int shakeVibrato = 15;
        [SerializeField] private float shakeRandomness = 90f;

        [Header("Bump")]
        [SerializeField] private float bumpDuration = 0.25f;
        [SerializeField] private Vector3 bumpPunch = new Vector3(0.25f, 0.25f, 0.25f);
        [SerializeField] private int bumpVibrato = 8;
        [SerializeField] private float bumpElasticity = 0.5f;

        [Header("Dealing")]
        [SerializeField] private float dealDuration = 0.35f;
        [SerializeField] private Vector2 dealStartOffset = new Vector2(0f, 400f);

        [Header("Disapear")]
        [SerializeField] private float disappearDuration = 0.2f;
        [SerializeField] private Ease disappearEase = Ease.InBack;

        [Header("Reference")]
        [SerializeField] private RectTransform _body;
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _group;

        private Tween _currentTween;
        private readonly Queue<Func<Tween>> _animationQueue = new();

        private void Awake()
        {
            if (_body == null)
            {
                _body = GetComponent<RectTransform>();
            }
            _body.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        private void OnDisable()
        {
            KillCurrent();
        }

        public void Flip(bool interruptAnimation = false)
        {
            Execute(() => FlipTween(true), true);
        }

        public void Unflip(bool interruptAnimation = false)
        {
            Execute(() => FlipTween(false), interruptAnimation);
        }

        private Tween FlipTween(bool flipToFront)
        {
            float halfDuration = flipDuration * 0.5f;
            if (flipToFront)
            {
                Sequence seq = DOTween.Sequence().SetTarget(_body);
                seq.Append(_body.DOLocalRotate(new Vector3(0f, 90f, 0f), halfDuration).SetEase(flipEase));
                seq.AppendCallback(() => { if (backObject != null) backObject.SetActive(true); });
                seq.Append(_body.DOLocalRotate(new Vector3(0f, 180f, 0f), halfDuration).SetEase(flipEase));
                return seq;
            }
            else
            {
                Sequence seq = DOTween.Sequence().SetTarget(_body);
                seq.Append(_body.DOLocalRotate(new Vector3(0f, 90f, 0f), halfDuration).SetEase(flipEase));
                seq.AppendCallback(() => { if (backObject != null) backObject.SetActive(false); });
                seq.Append(_body.DOLocalRotate(Vector3.zero, halfDuration).SetEase(flipEase));
                return seq;
            }
        }

        public void ShakeOnError(bool interruptAnimation = false)
        {
            Execute(() => _body.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness).SetTarget(_body), interruptAnimation);
        }

        public void BumpOnMatch(bool interruptAnimation = false)
        {
            Execute(() =>
            {
                var canvas = _root.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                return _body.DOPunchScale(bumpPunch, bumpDuration, bumpVibrato, bumpElasticity).SetTarget(_body).OnComplete(() =>
                {
                    Destroy(canvas);
                });
            }, interruptAnimation);

        }

        public void PlayDealAnimation(Action onDeal, bool interruptAnimation = false)
        {
            Execute(() =>
            {
                _group.alpha = 0;
                var startingPos = _body.anchoredPosition;
                _body.anchoredPosition -= dealStartOffset;
                Sequence seq = DOTween.Sequence().SetTarget(_root);
                seq.AppendInterval(UnityEngine.Random.value);
                seq.Append(_group.DOFade(1, dealDuration));
                seq.Join(_body.DOAnchorPos(startingPos, dealDuration));
                seq.JoinCallback(() => onDeal?.Invoke());
                return seq;
            }, interruptAnimation);
        }

        public void PlayDisappearAnimation(Action onComplete = null, bool interruptAnimation = false)
        {
            Execute(() =>
            {
                Vector3 zero = Vector3.zero;
                Tween t = _body.DOScale(zero, disappearDuration).SetEase(disappearEase).SetTarget(_body);
                if (onComplete != null)
                {
                    t.OnComplete(() => onComplete.Invoke());
                }
                return t;
            }, interruptAnimation);
        }

        private void Execute(Func<Tween> factory, bool interrupt)
        {
            if (interrupt)
            {
                KillCurrent();
            }

            if (_currentTween != null && _currentTween.IsActive() && _currentTween.IsPlaying())
            {
                _animationQueue.Enqueue(factory);
            }
            else
            {
                StartTween(factory);
            }
        }

        private void StartTween(Func<Tween> factory)
        {
            _currentTween = factory();
            _currentTween.OnComplete(DrainQueue);
        }

        private void DrainQueue()
        {
            _currentTween = null;
            if (_animationQueue.Count > 0)
            {
                var factory = _animationQueue.Dequeue();
                StartTween(factory);
            }
        }

        private void KillCurrent()
        {
            if (_body != null) DOTween.Kill(_body);
            if (_root != null) DOTween.Kill(_root);
            if (_group != null) DOTween.Kill(_group);
            _currentTween = null;
            _animationQueue.Clear();
        }

        public void KillAllTweens()
        {
            KillCurrent();
        }
    }
}
