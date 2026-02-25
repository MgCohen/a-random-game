using CardMatch.CardMatch;
using UnityEngine;

namespace CardMatch.Levels
{
    [CreateAssetMenu(fileName = "LevelRegistry", menuName = "CardMatch/Level Registry")]
    public class LevelRegistry : ScriptableObject
    {
        public Level[] Levels => levels;
        [SerializeField] private Level[] levels;
    }
}
