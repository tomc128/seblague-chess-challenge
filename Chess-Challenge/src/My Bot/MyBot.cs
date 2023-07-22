using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    // Again, thanks Sebastian for this snippet
    private readonly int[] _pieceValues = { 0, 100, 300, 300, 500, 900, 10_000 };

    public Move Think(Board board, Timer timer)
    {
        var isWhite = board.IsWhiteToMove;
        var allMoves = board.GetLegalMoves();

        var bestScore = -10_000;
        var chosenMove = allMoves.First();


        foreach (var move in allMoves)
        {
            board.MakeMove(move);
            var score = EvaluatePosition(board);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                chosenMove = move;
            }
        }

        return chosenMove;
    }

    private int EvaluatePosition(Board board)
    {
        if (board.IsInCheckmate()) return -10_000;
        if (board.IsDraw()) return 0;

        var isWhite = board.IsWhiteToMove;

        var score = 0;
        var pieceCounts = board.GetAllPieceLists().Select(x => x.Count).ToList();

        if (isWhite) pieceCounts.Reverse();
        for (var i = 0; i < 6; i++)
        {
            var above = pieceCounts[i];
            var below = pieceCounts[i + 6];

            score += _pieceValues[i] * (above - below);
        }

        return score;
    }

    // Test if this move gives checkmate
    // Taken from EvilBot.cs (thanks Sebastian)
    private bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        var isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}