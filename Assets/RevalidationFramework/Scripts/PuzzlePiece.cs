using UnityEngine;

namespace RevalidationFramework
{
    public class PuzzlePiece : MonoBehaviour
    {
        public int PieceID = 0;
        [HideInInspector]
        public PuzzleSlot PuzzleSlot { set; get; }

        private Vector3 startPosition;
        private Rigidbody _rigidbody;


        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            startPosition = transform.position;

        }

        public void Reset()
        {
            if (PuzzleSlot)
            {
                var puzzleSlotTransform = PuzzleSlot.transform;
                GoToPosition(puzzleSlotTransform.position, puzzleSlotTransform.rotation);
            }
            else
            {
                GoToPosition(startPosition, Quaternion.identity);
            }
        }

        public void GoToPosition(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;

            _rigidbody.velocity = Vector3.zero;

            _rigidbody.position = position;
            _rigidbody.rotation = rotation;
        }
    }
}
