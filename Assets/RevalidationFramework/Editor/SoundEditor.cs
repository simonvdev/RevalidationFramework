using UnityEditor;
using UnityEngine;

namespace RevalidationFramework
{
    [CustomEditor(typeof(Song))]
    public class SoundEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var script = (Song)target;

            if (GUILayout.Button("Reload Data", GUILayout.Height(40)))
            {
                script.LoadSong();
            }
        }
    }

}