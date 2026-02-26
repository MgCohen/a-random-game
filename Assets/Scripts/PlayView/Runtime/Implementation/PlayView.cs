using System;
using CardMatch.Audio;
using CardMatch.CardMatch;
using CardMatch.Navigation;
using CardMatch.PlaySystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.PlayView
{
    public class PlayView : View<PlayViewContext>
    {
        [SerializeField] private AudioService audioService;
        [SerializeField] private Board board;
        [SerializeField] private ScorePanel scorePanel;
        [SerializeField] private Button gameEndButton;
        [SerializeField] private Button returnButton;
        [SerializeField] private AudioClip gameOverClip;

        private Match match;

        protected override void OnShow()
        {
            OpenView();
        }

        protected override void OnHide()
        {
            CloseView();
        }

        protected override void OnClose()
        {
            CloseView();
        }

        private void OpenView()
        {
            match = Context?.Match;
            if (match == null)
            {
                throw new Exception("Trying to play without a match");
            }
            SetupMatch();
        }

        private void SetupMatch()
        {
            BindButtons();
            SetGameEndVisible(false);
            board.Open(match, Context.Level?.CardFrontColors);
            scorePanel.Open(match);
            match.Subscribe<MatchCompleted>(OnMatchCompleted);
        }

        public void OnMatchCompleted(MatchCompleted matchEvent)
        {
            if (gameOverClip != null && audioService != null)
                audioService.PlaySound(gameOverClip);
            SetGameEndVisible(true);
            Context.CompleteMatch();
        }

        private void CloseView()
        {
            if (gameEndButton != null && gameEndButton.transform is RectTransform gameEndRect)
                DOTween.Kill(gameEndRect, false);
            UnbindButtons();
            board.Close();
            scorePanel.Close();
            if (match != null)
            {
                match.Unsubscribe<MatchCompleted>(OnMatchCompleted);
                match.Dispose();
            }
            match = null;
        }

        private void SetGameEndVisible(bool visible)
        {
            gameEndButton.gameObject.SetActive(visible);
            RectTransform buttonRect = gameEndButton.transform as RectTransform;
            Vector2 anchoredPos = buttonRect.anchoredPosition;
            Vector2 initialPos = anchoredPos - new Vector2(0, 300);
            buttonRect.DOAnchorPos(anchoredPos, 0.3f).From(initialPos).SetTarget(buttonRect);
        }

        private void BindButtons()
        {
            returnButton.onClick.AddListener(HandleReturnClicked);
            gameEndButton.onClick.AddListener(HandleReturnClicked);
        }

        private void UnbindButtons()
        {
            returnButton.onClick.RemoveListener(HandleReturnClicked);
            gameEndButton.onClick.RemoveListener(HandleReturnClicked);
        }

        private void HandleReturnClicked()
        {
            Context.GoBack();
        }
    }
}
