using System.Collections.Generic;
using UnityEngine;

namespace RevalidationFramework
{
    [CreateAssetMenu(fileName = "NewBank", menuName = "RevalidationFramework/MidiSong/PianoSoundbank")]
    public class PianoSoundbank : ScriptableObject
    {
        public List<AudioClip> Notes = new List<AudioClip>();

    }
}

