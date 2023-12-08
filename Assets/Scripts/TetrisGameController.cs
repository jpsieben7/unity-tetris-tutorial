using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGameController : MonoBehaviour
{
    public Piece activePiece;
    public Board board;

    // Start is called before the first frame update
    private void Start()
    {
        SpawnPiece();
    }

    private void GameOver()
    {
        board.ClearAllTiles();

        // Do anything else you want on game over here..
    }

    private void SpawnPiece()
    {
        int random = Random.Range(0, board.tetrominoes.Length);
        TetrominoData data = board.tetrominoes[random];

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

    private void Step()
    {
        activePiece.stepTime = Time.time + activePiece.stepDelay;

        // Step down to the next row
        activePiece.Move(Vector2Int.down, board);

        // Once the piece has been inactive for too long it becomes locked
        if (activePiece.lockTime >= activePiece.lockDelay)
        {
            LockPieceAndSpawnNext();
        }
    }

    private void HardDrop()
    {
        while (activePiece.Move(Vector2Int.down, board))
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
            if (activePiece.Move(Vector2Int.down, board))
            {
                // Update the step time to prevent double movement
                activePiece.stepTime = Time.time + activePiece.stepDelay;
            }
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.A))
        {
            activePiece.Move(Vector2Int.left, board);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            activePiece.Move(Vector2Int.right, board);
        }
    }

    // Update is called once per frame
    private void Update()
    {
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
}
