using UnityEngine;

namespace CardMatch.CardMatch
{
    [CreateAssetMenu(fileName = "Level", menuName = "CardMatch/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private string levelId;
        [SerializeField] private LevelConfig config = new LevelConfig();
        [SerializeField] private Color[] cardFrontColors = System.Array.Empty<Color>();

        public string LevelId => levelId;
        public LevelConfig Config => config;
        public Color[] CardFrontColors => cardFrontColors;
    }
}
