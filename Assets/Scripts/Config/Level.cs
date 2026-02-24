using UnityEngine;

namespace CardMatch
{
    [CreateAssetMenu(fileName = "Level", menuName = "CardMatch/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private LevelConfig config = new LevelConfig();

        public LevelConfig Config => config;
    }
}
