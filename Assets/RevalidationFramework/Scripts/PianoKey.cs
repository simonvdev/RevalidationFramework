using UnityEngine;

namespace RevalidationFramework
{
    [System.Serializable]
    public struct KeyID
    {
        public string id;

        public int keyNumber;

    }

    [RequireComponent(typeof(AudioSource))]
    public class PianoKey : MonoBehaviour
    {
        public float activationEase = 1f;
        public float keyWidth = 0.02f;
        public float keyHeight = 0.02f;

        public KeyID PianoKeyID;


        public AudioClip NoteClip;

        public GameObject HighlightObject;

        private Piano _pianoInstance;

        private AudioSource _source;

        private HingeJoint _hingeJoint;
        private Rigidbody _rigidbody;
        private bool _isPressed = false;
        private bool _canBePlayed = false;


        private void Awake()
        {

            _source = GetComponent<AudioSource>();
            _source.spatialBlend = 1f;
            _hingeJoint = GetComponent<HingeJoint>();
            _rigidbody = GetComponent<Rigidbody>();

        }

        private void Start()
        {
            if (NoteClip && _source)
            {
                _source.clip = NoteClip;
            }
        }

        public void SetPianoReference(Piano pianoReference)
        {
            _pianoInstance = pianoReference;
        }

        private void FixedUpdate()
        {
            if (_hingeJoint.angle > (_hingeJoint.limits.max - activationEase) && !_isPressed)
            {
                _isPressed = true;

                if (_pianoInstance.GetMode() == PianoMode.TeachMode)
                {
                    if (!_canBePlayed) return;
                    
                    Play(1, false);

                    SetCanBePlayed(false);
                }
                else
                {
                    Play(1, false);

                }
            }
            else if (_hingeJoint.angle < 1)
            {
                _isPressed = false;
            }
        }

        public void Play(float volume, bool physically)
        {
            if (!physically)
            {
                if (_source)
                {
                    _source.Play();

                }
            }
            else
            {
                _rigidbody.AddForce(-transform.up * 6f);
            }
        }

        public void SetCanBePlayed(bool val)
        {
            _canBePlayed = val;
            HighlightObject.SetActive(val);

        }

        public bool GetMustBePlayed()
        {
            return _canBePlayed;
        }

    }
}