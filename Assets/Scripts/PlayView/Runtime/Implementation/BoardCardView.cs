using CardMatch.CardMatch;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.PlayView
{
    public class BoardCardView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image front;
        [SerializeField] private CardAnimator cardAnimator;

        private Card card;
        private Action<BoardCardView> onCardClicked;

        public Card Card => card;

        public void Bind(Card card, Color frontColor, Action<BoardCardView> onCardClicked)
        {
            this.card = card;
            this.onCardClicked = onCardClicked;
            if (front != null)
                front.color = frontColor;
            if (button != null)
                button.onClick.AddListener(OnClick);
            //PlayStateAnimation(card.State);
        }

        public void PlayStateAnimation(CardState state, bool interruptAnimation = false)
        {
            if (cardAnimator != null)
            {
                if (state == CardState.Hidden) Hide(interruptAnimation);
                else if (state == CardState.Flipped) Flip(interruptAnimation);
                else if (state == CardState.Scored) Bump(interruptAnimation);
            }
        }

        public void PlayDealAnimation(Action onDeal, bool interruptAnimation = false)
        {
            cardAnimator?.PlayDealAnimation(onDeal, interruptAnimation);
        }

        private void OnClick()
        {
            onCardClicked?.Invoke(this);
        }

        private void Hide(bool interrupt)
        {
            cardAnimator.ShakeOnError(interrupt);
            cardAnimator.Unflip(interrupt);
        }

        private void Flip(bool interrupt)
        {
            cardAnimator.Flip(interrupt);
        }

        private void Bump(bool interrupt)
        {
            cardAnimator.BumpOnMatch(interrupt);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClick);
        }
    }
}
