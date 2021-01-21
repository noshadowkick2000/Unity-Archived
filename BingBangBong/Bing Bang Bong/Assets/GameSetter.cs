using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetter : MonoBehaviour
{
    private char[] fileArray = {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};

    [SerializeField] private float gridSize;
    [SerializeField] private float animationTime;
    [SerializeField] private float slowTimeMultiplier;

    private Manager manager;
    private AudioSource sound;

    /*
    [SerializeField] private GameObject whitePawn;
    [SerializeField] private GameObject whiteBishop;
    [SerializeField] private GameObject whiteKnight;
    [SerializeField] private GameObject whiteRook;
    [SerializeField] private GameObject whiteQueen;
    [SerializeField] private GameObject whiteKing;
    [SerializeField] private GameObject blackPawn;
    [SerializeField] private GameObject blackBishop;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject blackRook;
    [SerializeField] private GameObject blackQueen;
    [SerializeField] private GameObject blackKing;
    */

    [Serializable]
    public struct PieceLocation
    {
        public GameObject piece;
        public String position;
    }

    [Serializable]
    public struct PieceMove
    {
        public PieceLocation oldPosition;
        public String newPosition;
    }

    [Serializable]
    public struct Position
    {
        public String details;
        public PieceLocation[] pieces;
        public PieceMove movingPiece;
        public bool mate;
    }

    [SerializeField] public Position[] positions;

    private List<GameObject> activePieces = new List<GameObject>();
    private int index;

    private void Awake()
    {
        manager = FindObjectOfType<Manager>();
        sound = GetComponent<AudioSource>();
    }

    private Vector3 GetPosition(string position)
    {
        if (position.Length != 2)
            return Vector3.back;
        
        int x = 0;
        int counter = 0;
        foreach (var file in fileArray)
        {
            if (file.Equals(position[0]))
                x = counter;
            counter++;
        }
        int z = (int)Char.GetNumericValue(position[1]) - 1;
        
        return new Vector3(x, 0, z);
    }

    public IEnumerator MovePiece(Vector3 goalPosition, bool slow)
    {
        Vector3 startPosition = GetPosition(positions[index].movingPiece.oldPosition.position);
        float startTime = Time.time;
        float endTime = slow ? (startTime + animationTime*slowTimeMultiplier) : (startTime + animationTime);
        while (Time.time < endTime)
        {
            // Interpolate move piece
            float t = slow ? (Time.time - startTime) / (animationTime*slowTimeMultiplier) : (Time.time - startTime) / animationTime;
            activePieces[0].transform.position =
                Vector3.Lerp(startPosition, goalPosition, t);
            yield return null;
        }

        activePieces[0].transform.position = goalPosition;
        
        sound.Play();

        if (!slow)
            manager.Moved(positions[index].mate);
    }

    public void SetRandomBoard(Text description)
    {
        int i = UnityEngine.Random.Range(0, positions.Length);
        SetBoard(i, description);
    }

    public void SetNextBoard(Text description)
    {
        index++;
        if (index >= positions.Length)
            index = 0;
        SetBoard(index, description);
    }

    private void SetBoard(int index, Text description)
    {
        if (index >= positions.Length) return;
        if (activePieces.Count > 0) ClearBoard();
        this.index = index;
        // Setup the moving piece and cache game object
        // Always use first object of the list as moving piece
        activePieces.Add(Instantiate(positions[index].movingPiece.oldPosition.piece, gridSize * GetPosition(positions[index].movingPiece.oldPosition.position), positions[index].movingPiece.oldPosition.piece.transform.rotation));
        
        // Setup all non moving pieces and cache game objects
        foreach (var piecePositions in positions[index].pieces)
        {
            activePieces.Add(Instantiate(piecePositions.piece, gridSize * GetPosition(piecePositions.position), piecePositions.piece.transform.rotation));
        }

        if (description!=null)
            description.text = positions[index].details;
    }

    public void ResetPieces()
    {
        activePieces[0].transform.position = gridSize * GetPosition(positions[index].movingPiece.oldPosition.position);
        int counter = 1;
        foreach (var piecePositions in positions[index].pieces)
        {
            activePieces[counter].transform.position = gridSize * GetPosition(piecePositions.position);
            counter++;
        }
    }

    public void ClearBoard()
    {
        StopAllCoroutines();
        foreach (var piece in activePieces)
        {
            Destroy(piece);
        }
        activePieces.Clear();
    }

    public Transform GetMovingPieceTransform()
    {
        return activePieces.Count > 0 ? activePieces[0].transform : null;
    }

    public void MovePiece(bool slow)
    {
        StartCoroutine(MovePiece(GetPosition(positions[index].movingPiece.newPosition), slow));
    }
}
