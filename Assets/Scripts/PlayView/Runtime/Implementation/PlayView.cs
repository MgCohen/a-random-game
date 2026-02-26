using System;
using CardMatch.Audio;
using CardMatch.CardMatch;
using CardMatch.Navigation;
using CardMatch.PlaySystem;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatch.PlayView
{
    public class PlayView : View<PlayViewContext>
    {
        [SerializeField] private AudioService audioService;
        [SerializeField] private Board board;
        [SerializeField] private ScorePanel scorePanel;
        [SerializeField] private GameObject gameEndPopup;
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
            if (gameEndPopup == null)
            {
                throw new Exception("Missing end game popup");
            }
            gameEndPopup.SetActive(visible);
        }

        private void BindButtons()
        {
            returnButton.onClick.AddListener(HandleReturnClicked);
        }

        private void UnbindButtons()
        {
            returnButton.onClick.RemoveListener(HandleReturnClicked);
        }

        private void HandleReturnClicked()
        {
            Context.GoBack();
        }
    }
}
