using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RevalidationFramework
{
    public class Puzzle : MonoBehaviour
    {
        public List<PuzzleSlot> _puzzleSlots = new List<PuzzleSlot>();

        public bool isSolved = false;

        private void Start()
        {
            foreach (var puzzleSlot in _puzzleSlots)
            {
                puzzleSlot.puzzleReference = this;
            }
        }

        public void CheckPuzzle()
        {
            int amountOfPiecesSolved = _puzzleSlots.Count(t => t.IsFilled);
            isSolved = amountOfPiecesSolved == _puzzleSlots.Count;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("PuzzlePiece")) return;
            
            PuzzlePiece puzzlePiece = other.GetComponent<PuzzlePiece>();
            puzzlePiece.Reset();
        }
    }

}

