using UnityEditor;
using UnityEngine;

namespace CardMatch.Persistence.Editor
{
    public static class ClearPlayerPrefsMenuItem
    {
        [MenuItem("Tools/Clear Player Prefs")]
        public static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("Player prefs cleared.");
        }
    }
}
