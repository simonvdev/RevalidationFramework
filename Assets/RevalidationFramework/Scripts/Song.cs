using System.Collections.Generic;
using UnityEngine;

namespace RevalidationFramework
{
    [CreateAssetMenu(fileName = "NewSong", menuName = "RevalidationFramework/MidiSong/Song")]
    public class Song : ScriptableObject
    {
        public string SongName = "SampleName";
        public string ArtistName = "Sample Artist";
        public TextAsset _midiAsset;


        public float BPM = 10;

        private int _PPQ = 0;
        private int _trackCount = 0;
        private float _msPerTick = 0;

        private MidiFile _midiData;

        private float[] _trackTimers;
        private int[] _trackIndexs;

        [HideInInspector]
        public List<int> NotesToPlay => _currentNotes;

        private List<int> _currentNotes;

        public void LoadSong()
        {
            _midiData = new MidiFile(_midiAsset);
            if (_midiData == null)
                Debug.LogError("NO MIDI TEXT ASSET ASSIGNED");

            _PPQ = _midiData.TicksPerQuarterNote;

            _msPerTick = (60f / (_PPQ * BPM));

            _trackCount = _midiData.TracksCount;

            _trackTimers = new float[_trackCount];
            _trackIndexs = new int[_trackCount];
            _currentNotes = new List<int>();
        }

        public void UpdateSong()
        {
            _currentNotes.Clear();

            if (_midiAsset == null)
                Debug.LogError("NO MIDI TEXT ASSET ASSIGNED");

            if (_midiData == null)
                return;

            for (int i = 0; i < _trackCount; i++)
            {
                if (_trackIndexs[i] >= _midiData.Tracks[i].MidiEvents.Count || _trackIndexs[i] < 0) continue;
                
                MidiEvent midiEvent = _midiData.Tracks[i].MidiEvents[_trackIndexs[i]];

                _trackTimers[i] += Time.deltaTime;
                if (!(_trackTimers[i] >= midiEvent.Time * _msPerTick)) continue;
                
                _trackIndexs[i]++;
                
                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    _currentNotes.Add(midiEvent.Note);
                }
            }
        }

        public void ResetSong()
        {
            _currentNotes.Clear();

            _trackTimers = new float[_trackCount];
            _trackIndexs = new int[_trackCount];
        }
    }
}

