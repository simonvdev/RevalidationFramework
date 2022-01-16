using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RevalidationFramework
{
    public enum PianoMode
    {
        PreviewMode,
        TeachMode,
        FreeMode
    }

    public class Piano : MonoBehaviour
    {
        public int amountOfKeys = 52;

        public char startNote = 'a';

        public PianoKey whiteKeyPrefab;
        public PianoKey blackKeyPrefab;

        public PianoSoundbank soundbank;

        public List<Song> songsToPlay = new List<Song>();

        [Header("UI Variables")]
        public Text songNameText;
        public Text artistNameText;


        private int _currentSongIndex = 0;

        private PianoMode _pianoMode = PianoMode.TeachMode;

        private float _keyWidth = 0f;
        private float _keyHeight = 0f;

        private readonly List<PianoKey> _keys = new List<PianoKey>();

        bool _isPlaying = true;
        bool _canUpdate = true;

        private void Awake()
        {
            GenerateKeys();

        }

        void Start()
        {

            foreach (var song in songsToPlay)
            {
                song.LoadSong();
            }

            UpdateUIData();

            foreach (var key in _keys)
            {
                key.SetPianoReference(this);
            }

            SetPianoMode(PianoMode.TeachMode);
        }

        public PianoMode GetMode()
        {
            return _pianoMode;
        }

        public void SetPianoMode(PianoMode mode)
        {
            _pianoMode = mode;
            ResetSong();

            switch (_pianoMode)
            {
                case PianoMode.PreviewMode:
                    SetPlayState(true);
                    break;
                case PianoMode.TeachMode:
                    break;
                case PianoMode.FreeMode:
                    break;
            }

            for (int i = 0; i < _keys.Count; i++)
            {
                _keys[i].SetCanBePlayed(false);
            }
        }
        public void SetPianoMode(int modeNumber)
        {
            SetPianoMode((PianoMode)modeNumber);
        }

        void GenerateKeys()
        {
            var localScale = transform.localScale;
            _keyWidth = whiteKeyPrefab.keyWidth * localScale.x;
            _keyHeight = whiteKeyPrefab.keyHeight * localScale.y;

            KeyID keyID = new KeyID();
            char currentNote = startNote;
            keyID.keyNumber = 0;

            Vector3 currentSpawnPos = transform.position;

            for (int i = 0; i < amountOfKeys; i++)
            {
                var whiteKeyInstance = Instantiate(whiteKeyPrefab, currentSpawnPos, Quaternion.identity, transform);

                keyID.id = $"{currentNote}{keyID.keyNumber}";

                whiteKeyInstance.name = keyID.id;
                whiteKeyInstance.PianoKeyID = keyID;
                whiteKeyInstance.NoteClip = soundbank.Notes.Find(x => string.Equals(x.name, keyID.id, System.StringComparison.OrdinalIgnoreCase));
                _keys.Add(whiteKeyInstance);

                if (currentNote != 'b' && currentNote != 'e' && i != amountOfKeys - 1)
                {
                    Vector3 blackKeySpawnPos = new Vector3(currentSpawnPos.x - (_keyWidth / 2f), currentSpawnPos.y + _keyHeight / 2f, currentSpawnPos.z);
                    
                    var blackKeyInstance = Instantiate(blackKeyPrefab, blackKeySpawnPos, Quaternion.identity, transform);

                    keyID.id = $"{currentNote}-{keyID.keyNumber}";

                    blackKeyInstance.name = keyID.id;
                    blackKeyInstance.PianoKeyID = keyID;
                    blackKeyInstance.NoteClip = soundbank.Notes.Find(x => string.Equals(x.name, keyID.id, System.StringComparison.OrdinalIgnoreCase));
                    _keys.Add(blackKeyInstance);
                }
                currentSpawnPos.x -= _keyWidth;
                currentNote++;

                if (currentNote > 'g')
                {
                    currentNote = 'a';
                }
                if (currentNote == 'c')
                {
                    keyID.keyNumber++;
                }
            }
        }

        private void Update()
        {
            switch (_pianoMode)
            {
                case PianoMode.PreviewMode:
                    PlaySong();
                    break;
                case PianoMode.TeachMode:

                    TeachSong();
                    break;
                case PianoMode.FreeMode:
                    break;
                default:
                    break;
            }
        }

        private void PlaySong()
        {
            if (songsToPlay.Count == 0) return;
            if (!_isPlaying) return;
            songsToPlay[_currentSongIndex].UpdateSong();
            foreach (int note in songsToPlay[_currentSongIndex].NotesToPlay)
            {
                int playableNote = note - 24;

                if (playableNote < _keys.Count && playableNote >= 0)
                {
                    _keys[playableNote].Play(0.5f, true);
                }
            }
        }

        public void ResetSong()
        {
            songsToPlay[_currentSongIndex].ResetSong();
            SetPlayState(false);
        }

        public void ChooseSong(bool forward)
        {
            if (forward)
            {
                ResetSong();
                _currentSongIndex++;
                if (_currentSongIndex >= songsToPlay.Count)
                {
                    _currentSongIndex = 0;
                }
            }
            else
            {
                ResetSong();
                _currentSongIndex--;
                if (_currentSongIndex < 0)
                {
                    _currentSongIndex = songsToPlay.Count - 1;
                }
            }
            UpdateUIData();
            SetPlayState(true);
        }

        public void UpdateUIData()
        {
            songNameText.text = "Current Song: " + songsToPlay[_currentSongIndex].SongName;
            artistNameText.text = "Current Artist: " + songsToPlay[_currentSongIndex].ArtistName;
        }

        public void SetPlayState(bool value)
        {
            _isPlaying = value;
        }

        private void TeachSong()
        {
            if (songsToPlay.Count == 0) return;
            
            if (_canUpdate)
            {
                _canUpdate = false;
                songsToPlay[_currentSongIndex].UpdateSong();

                if (songsToPlay[_currentSongIndex].NotesToPlay.Count == 0)
                {
                    _canUpdate = true;
                }

                foreach (int note in songsToPlay[_currentSongIndex].NotesToPlay)
                {
                    int playableNote = note - 24;

                    if (playableNote < _keys.Count && playableNote >= 0)
                    {
                        _keys[playableNote].SetCanBePlayed(true);
                    }
                }
            }
            else
            {
                int amountOfNotesPlayed = 0;

                foreach (int note in songsToPlay[_currentSongIndex].NotesToPlay)
                {
                    int playableNote = note - 24;

                    if (playableNote < _keys.Count && playableNote >= 0)
                    {
                        if (!_keys[playableNote].GetMustBePlayed()) amountOfNotesPlayed++;
                    }
                }
                _canUpdate = amountOfNotesPlayed == songsToPlay[_currentSongIndex].NotesToPlay.Count;
            }
        }
    }
}