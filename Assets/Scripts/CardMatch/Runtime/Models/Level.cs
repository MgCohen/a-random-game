using UnityEngine;

namespace CardMatch.CardMatch
{
    [CreateAssetMenu(fileName = "Level", menuName = "CardMatch/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private string levelId;
        [SerializeField] private LevelConfig config = new LevelConfig();
        [SerializeField] private Sprite cardBack;
        [SerializeField] private Sprite cardFrontBase;
        [SerializeField] private Color[] cardFrontColors = System.Array.Empty<Color>();

        public string LevelId => levelId;
        public LevelConfig Config => config;
        public Sprite CardBack => cardBack;
        public Sprite CardFrontBase => cardFrontBase;
        public Color[] CardFrontColors => cardFrontColors;
    }
}
