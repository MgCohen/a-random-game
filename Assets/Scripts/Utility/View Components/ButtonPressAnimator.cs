using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>Drop-on utility that animates a RectTransform (e.g. a button) with DOTween: slight Y-down on hover, press-down then pop on click.</summary>
[RequireComponent(typeof(RectTransform))]
public class ButtonPressAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverOffsetY = 4f;
    [SerializeField] private float clickOffsetY = 8f;
    [SerializeField] private float hoverDuration = 0.1f;
    [SerializeField] private float pressDuration = 0.05f;
    [SerializeField] private float popDuration = 0.15f;

    private RectTransform _rect;
    private Vector2 _initialAnchoredPos;
    private bool _isHovered;
    private Tween _positionTween;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _initialAnchoredPos = _rect.anchoredPosition;
    }

    private void OnDisable()
    {
        KillPositionTween();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        KillPositionTween();
        TweenToHoverRest();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        KillPositionTween();
        TweenToFullRest();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        KillPositionTween();
        TweenToPressDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        KillPositionTween();
        TweenToRestWithPop();
    }

    private void KillPositionTween()
    {
        if (_positionTween != null && _positionTween.IsActive())
        {
            _positionTween.Kill();
            _positionTween = null;
        }
    }

    private float GetRestY()
    {
        float offset = _isHovered ? hoverOffsetY : 0f;
        return _initialAnchoredPos.y - offset;
    }

    private void TweenToHoverRest()
    {
        float targetY = _initialAnchoredPos.y - hoverOffsetY;
        _positionTween = _rect.DOAnchorPosY(targetY, hoverDuration).SetEase(Ease.OutQuad);
    }

    private void TweenToFullRest()
    {
        _positionTween = _rect.DOAnchorPosY(_initialAnchoredPos.y, hoverDuration).SetEase(Ease.OutQuad);
    }

    private void TweenToPressDown()
    {
        float targetY = _initialAnchoredPos.y - clickOffsetY;
        _positionTween = _rect.DOAnchorPosY(targetY, pressDuration).SetEase(Ease.OutQuad);
    }

    private void TweenToRestWithPop()
    {
        float targetY = GetRestY();
        _positionTween = _rect.DOAnchorPosY(targetY, popDuration).SetEase(Ease.OutBack);
    }
}
