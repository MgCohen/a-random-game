using CardMatch.CardMatch;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace CardMatch.PlayView
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI roundText;

        private Match match;

        public void Open(Match matchToBind)
        {
            match = matchToBind;
            if (match == null) return;
            GameState state = match.CurrentState;
            UpdateScoreValue(state.Score);
            UpdateRoundValue(state.Round);
            match.Subscribe<ScoreChanged>(OnScoreChanged);
            match.Subscribe<RoundChanged>(OnRoundChanged);
        }

        private void OnRoundChanged(RoundChanged e)
        {
            UpdateRoundValue(e.Round);
        }

        private void UpdateRoundValue(int value)
        {
            roundText.text = value.ToString();
        }

        private void OnScoreChanged(ScoreChanged e)
        {
            UpdateScoreValue(e.Score); 
        }

        private void UpdateScoreValue(int value)
        {
            scoreText.text = value.ToString();
        }

        public void Close()
        {
            match.Unsubscribe<ScoreChanged>(OnScoreChanged);
            match.Unsubscribe<RoundChanged>(OnRoundChanged);
            match = null;
        }
    }
}
