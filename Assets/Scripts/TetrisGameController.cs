using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using UnityEngine;

public class TetrisGameController : MonoBehaviour
{
    public const int previewTileSpacing = 1;
    public Piece activePiece;
    public Board board;
    public Board previewBoard;
    public Piece piecePrefab;
    public List<Piece> previewPieces = new List<Piece>();
    
    private const int previewTileOffset = previewTileSpacing + 1;

    // Start is called before the first frame update
    private void Start()
    {
        SpawnPiece();

        InitPreviewBoard();
    }

    private void InitPreviewBoard() {
        //Spawn all the preview pieces
        for (int i = 0; i < 4; ++i) {
            Piece newPiece = Instantiate(piecePrefab);
            TetrominoData data = GenerateRandomTetromino();
            newPiece.Initialize(previewBoard.spawnPosition, data);

            previewPieces.Add(newPiece);
            previewBoard.Set(newPiece);
        }

        //Clear them all from the board beforehand so that clearing doesn't interfere with the newly set pieces
        foreach(Piece item in previewPieces) { previewBoard.Clear(item); }

        //Move them into place
        int moveAmount = 0;
        foreach(Piece item in previewPieces) {
            //Move the piece the current move amount then set the new move amount based on its new position
            item.Move(Vector2Int.down * moveAmount, previewBoard);
            BoundsInt bounds = item.GetBoundsInt(false);
            int height = bounds.yMax - bounds.yMin;
            item.Move(Vector2Int.down * height, previewBoard);
            previewBoard.Set(item);
            //We start at a high y-value, so we need to move it down the amount it has already moved plus an additional offset
            //TODO: Working, but current problem is that we aren't considering tile height in the move amount
            moveAmount = (previewBoard.spawnPosition.y - item.GetBoundsInt().yMin) + previewTileOffset;
        }
    }

    private void GameOver()
    {
        board.ClearAllTiles();

        // Do anything else you want on game over here..
    }

    private TetrominoData GenerateRandomTetromino() {
        int random = Random.Range(0, board.tetrominoes.Length);
        return board.tetrominoes[random];
    }

    private void SpawnPiece()
    {
        TetrominoData data = GenerateRandomTetromino();

        activePiece.Initialize(board.spawnPosition, data);

        if (board.IsValidPosition(activePiece, board.spawnPosition))
        {
            board.Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    private void LockPieceAndSpawnNext()
    {
        board.LockInBoard(activePiece);
        SpawnPiece();
    }

    private bool MoveActivePiece(Vector2Int translation) {
        return activePiece.Move(translation, board);
    }

    private void Step()
    {
        activePiece.stepTime = Time.time + activePiece.stepDelay;

        // Step down to the next row
        MoveActivePiece(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (activePiece.lockTime >= activePiece.lockDelay)
        {
            LockPieceAndSpawnNext();
        }
    }

    private void HardDrop()
    {
        while (MoveActivePiece(Vector2Int.down))
        {
            continue;
        }

        LockPieceAndSpawnNext();
    }

    private void HandleMoveInputs()
    {
        // Soft drop movement
        if (Input.GetKey(KeyCode.S))
        {
            if (MoveActivePiece(Vector2Int.down))
            {
                // Update the step time to prevent double movement
                activePiece.stepTime = Time.time + activePiece.stepDelay;
            }
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.A))
        {
            MoveActivePiece(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveActivePiece(Vector2Int.right);
        }
    }

    private void UpdateBoard() {
        board.Clear(activePiece);

        // We use a timer to allow the player to make adjustments to the piece
        // before it locks in place
        activePiece.lockTime += Time.deltaTime;

        // Handle rotation
        if (Input.GetKeyDown(KeyCode.Q))
        {
            activePiece.Rotate(-1, board);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            activePiece.Rotate(1, board);
        }

        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > activePiece.moveTime)
        {
            HandleMoveInputs();
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > activePiece.stepTime)
        {
            Step();
        }

        board.Set(activePiece);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateBoard();
    }
}
