using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevalidationFramework
{
    [RequireComponent(typeof(AudioSource))]
    public class PuzzleSlot : MonoBehaviour
    {
        public Puzzle puzzleReference { set; get; }

        public float distanceToTarget = 0.01f;
        [Range(0, 1)]
        public float rotationRange = 0.1f;
        public int PieceID = 0;
        [HideInInspector]
        public bool IsFilled { get { return _isFilled; } }
        private bool _isFilled = false;

        public AudioClip audioClip;


        PuzzlePiece pieceInSlot = null;

        private MeshRenderer _meshRenderer;
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();


            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshRenderer == null) Debug.LogError("NO MESHRENDERER ASSIGNED!!!");
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsFilled) return;

            if (other.CompareTag("PuzzlePiece"))
            {
                PuzzlePiece piece = other.GetComponent<PuzzlePiece>();
                if (piece.PieceID != PieceID) return;

                Vector3 targetToSlot = transform.position - other.transform.position;
                if (targetToSlot.sqrMagnitude < distanceToTarget * distanceToTarget)
                {
                    pieceInSlot = piece;

                    other.transform.rotation = transform.rotation;
                    other.transform.position = transform.position;

                    other.attachedRigidbody.isKinematic = true;

                    piece.PuzzleSlot = this;
                    _meshRenderer.enabled = false;
                    _isFilled = true;

                    LerpToSlot();

                    //puzzleReference.CheckPuzzle();

                    _audioSource.PlayOneShot(audioClip, 0.8f);
                }
            }
        }

        private IEnumerator LerpToSlot()
        {
            float elapsedTime = 0f;
            while (elapsedTime < 2.0f)
            {
                pieceInSlot.transform.position = Vector3.Lerp(pieceInSlot.transform.position, transform.position, elapsedTime / 2.0f);
                pieceInSlot.transform.rotation = Quaternion.Lerp(pieceInSlot.transform.rotation, transform.rotation, elapsedTime / 2.0f);
                elapsedTime += Time.deltaTime;

                // Yield here
                yield return null;
            }
            // Make sure we got there
            pieceInSlot.transform.position = transform.position;
            pieceInSlot.transform.rotation = transform.rotation;
            yield return null;
        }

        private bool CompareOriententions(Quaternion quatA, Quaternion value, float acceptableRange)
        {
            return 1 - Mathf.Abs(Quaternion.Dot(quatA.normalized, value.normalized)) < acceptableRange;
        }
    }
}

