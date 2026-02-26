using System.Collections;
using System.Collections.Generic;
using CardMatch.Audio;
using CardMatch.CardMatch;
using UnityEngine;

namespace CardMatch.PlayView
{
    public class Board : MonoBehaviour
    {
        [SerializeField] private AudioService audioService;
        [SerializeField] private AudioClip cardFlipClip;
        [SerializeField] private AudioClip cardMatchClip;
        [SerializeField] private AudioClip cardMismatchClip;
        [SerializeField] private AudioClip cardDealClip;
        [SerializeField] private RectTransform cardContainer;
        [SerializeField] private BoardLayout boardLayout;
        [SerializeField] private BoardCardView cardPrefab;
        [SerializeField]
        private Color[] cardPalette = new Color[]
        {
            new Color(1f, 0.4f, 0.4f),
            new Color(0.4f, 0.6f, 1f),
            new Color(0.5f, 0.9f, 0.5f),
            new Color(1f, 0.85f, 0.4f),
            new Color(0.9f, 0.5f, 0.9f),
            new Color(0.4f, 0.9f, 0.9f),
        };
        [SerializeField] private float dealStaggerDelay = 0.05f;

        private Match match;
        private Dictionary<Card, BoardCardView> cardViews = new();
        private Dictionary<int, Color> _colorCache = new();
        private Color[] _levelPalette;

        public void Open(Match match, Color[] levelPalette = null)
        {
            if (match == null) return;
            this.match = match;
            _levelPalette = (levelPalette != null && levelPalette.Length > 0) ? levelPalette : null;
            ClearCards();
            Setup();
        }

        private void Setup()
        {
            GameState state = this.match.CurrentState;
            if (state?.Layout != null)
                boardLayout?.Configure(state.Layout.Rows, state.Layout.Columns);

            SpawnBoard();
            this.match.Subscribe<CardStateChanged>(OnCardStateChanged);
            this.match.Subscribe<CardsMatched>(OnCardsMatched);
            this.match.Subscribe<CardsMismatched>(OnCardsMismatched);
            StartCoroutine(PlayDealAnimationsWhenReady());
        }

        private void SpawnBoard()
        {
            GameState state = this.match.CurrentState;
            if (state?.Cards == null || state.Layout == null) return;
            int slotCount = state.Layout.Rows * state.Layout.Columns;
            for (int i = 0; i < slotCount && i < state.Cards.Count; i++)
            {
                Card card = state.Cards[i];
                SpawnCard(card);
            }
        }

        private Color ColorForCardId(int cardId)
        {
            Color[] palette = (_levelPalette != null && _levelPalette.Length > 0) ? _levelPalette : cardPalette;
            if (palette == null || palette.Length == 0)
                return Color.gray;
            if (cardId < palette.Length)
                return palette[cardId];
            if (_colorCache.TryGetValue(cardId, out Color cached))
                return cached;
            Color generated = DeterministicColorForCardId(cardId);
            _colorCache[cardId] = generated;
            return generated;
        }

        private static Color DeterministicColorForCardId(int cardId)
        {
            var rng = new System.Random(cardId);
            float r = 0.3f + (float)rng.NextDouble() * 0.7f;
            float g = 0.3f + (float)rng.NextDouble() * 0.7f;
            float b = 0.3f + (float)rng.NextDouble() * 0.7f;
            return new Color(r, g, b);
        }

        private void SpawnCard(Card card)
        {
            BoardCardView view = Instantiate(cardPrefab, cardContainer);
            view.Bind(card, ColorForCardId(card.CardId), OnCardClicked);
            cardViews[card] = view;
        }

        private IEnumerator PlayDealAnimationsWhenReady()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            if (cardContainer != null)
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(cardContainer);

            foreach (var card in cardViews)
            {
                card.Value.PlayDealAnimation(() => audioService.PlaySound(cardDealClip), false);
            }
        }

        private void OnCardClicked(BoardCardView view)
        {
            match.FlipCard(view.Card);
        }

        private void OnCardStateChanged(CardStateChanged e)
        {
            if (e.State == CardState.Flipped && cardFlipClip != null && audioService != null)
                audioService.PlaySound(cardFlipClip);
            if (cardViews.TryGetValue(e.Card, out BoardCardView cardView))
            {
                cardView.PlayStateAnimation(e.State, interruptAnimation: false);
            }
        }

        private void OnCardsMatched(CardsMatched e)
        {
            if (cardMatchClip != null && audioService != null)
                audioService.PlaySound(cardMatchClip);
        }

        private void OnCardsMismatched(CardsMismatched e)
        {
            if (cardMismatchClip != null && audioService != null)
                audioService.PlaySound(cardMismatchClip);
        }

        public void Close()
        {
            match.Unsubscribe<CardStateChanged>(OnCardStateChanged);
            match.Unsubscribe<CardsMatched>(OnCardsMatched);
            match.Unsubscribe<CardsMismatched>(OnCardsMismatched);
            match = null;
            ClearCards();
        }

        private void ClearCards()
        {
            foreach (BoardCardView v in cardViews.Values)
            {
                if (v != null && v.gameObject != null)
                    Destroy(v.gameObject);
            }
            cardViews.Clear();
            _colorCache.Clear();
            _levelPalette = null;
        }
    }
}
