using FluentAssertions;
using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Retrodactyl.Chess.Core.UnitTests
{
    public class BoardTests
    {
        [Theory]
        [InlineData(0, 0, Player.Black, PieceType.Rook)]
        [InlineData(1, 0, Player.Black, PieceType.Knight)]
        [InlineData(2, 0, Player.Black, PieceType.Bishop)]
        [InlineData(3, 0, Player.Black, PieceType.Queen)]
        [InlineData(4, 0, Player.Black, PieceType.King)]
        [InlineData(5, 0, Player.Black, PieceType.Bishop)]
        [InlineData(6, 0, Player.Black, PieceType.Knight)]
        [InlineData(7, 0, Player.Black, PieceType.Rook)]
        [InlineData(0, 1, Player.Black, PieceType.Pawn)]
        [InlineData(1, 1, Player.Black, PieceType.Pawn)]
        [InlineData(2, 1, Player.Black, PieceType.Pawn)]
        [InlineData(3, 1, Player.Black, PieceType.Pawn)]
        [InlineData(4, 1, Player.Black, PieceType.Pawn)]
        [InlineData(5, 1, Player.Black, PieceType.Pawn)]
        [InlineData(6, 1, Player.Black, PieceType.Pawn)]
        [InlineData(7, 1, Player.Black, PieceType.Pawn)]
        [InlineData(0, 2)]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        [InlineData(3, 2)]
        [InlineData(4, 2)]
        [InlineData(5, 2)]
        [InlineData(6, 2)]
        [InlineData(7, 2)]
        [InlineData(0, 3)]
        [InlineData(1, 3)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(4, 3)]
        [InlineData(5, 3)]
        [InlineData(6, 3)]
        [InlineData(7, 3)]
        [InlineData(0, 4)]
        [InlineData(1, 4)]
        [InlineData(2, 4)]
        [InlineData(3, 4)]
        [InlineData(4, 4)]
        [InlineData(5, 4)]
        [InlineData(6, 4)]
        [InlineData(7, 4)]
        [InlineData(0, 5)]
        [InlineData(1, 5)]
        [InlineData(2, 5)]
        [InlineData(3, 5)]
        [InlineData(4, 5)]
        [InlineData(5, 5)]
        [InlineData(6, 5)]
        [InlineData(7, 5)]
        [InlineData(0, 6, Player.White, PieceType.Pawn)]
        [InlineData(1, 6, Player.White, PieceType.Pawn)]
        [InlineData(2, 6, Player.White, PieceType.Pawn)]
        [InlineData(3, 6, Player.White, PieceType.Pawn)]
        [InlineData(4, 6, Player.White, PieceType.Pawn)]
        [InlineData(5, 6, Player.White, PieceType.Pawn)]
        [InlineData(6, 6, Player.White, PieceType.Pawn)]
        [InlineData(7, 6, Player.White, PieceType.Pawn)]
        [InlineData(0, 7, Player.White, PieceType.Rook)]
        [InlineData(1, 7, Player.White, PieceType.Knight)]
        [InlineData(2, 7, Player.White, PieceType.Bishop)]
        [InlineData(3, 7, Player.White, PieceType.Queen)]
        [InlineData(4, 7, Player.White, PieceType.King)]
        [InlineData(5, 7, Player.White, PieceType.Bishop)]
        [InlineData(6, 7, Player.White, PieceType.Knight)]
        [InlineData(7, 7, Player.White, PieceType.Rook)]
        public void ShouldBeSetupWithStartingPositions(int x, int y, Player expectedPieceColor = default(Player), PieceType expectedPieceType = default(PieceType))
        {
            var board = new Board();
            var piece = board[y, x];

            if (expectedPieceType == PieceType.None && expectedPieceColor == Player.None)
            {
                piece.Should().BeNull();
            }
            else
            {
                piece.Should().NotBeNull();
                piece.location.Should().Be(new Square(x, y));
                piece.player.Should().Be(expectedPieceColor);
                piece.type.Should().Be(expectedPieceType);
                piece.moveCount.Should().Be(0);
            }
        }


        [Fact]
        public void ShouldAllowSearchingValidMoves()
        {
            var board = new Board();
            // with the default starting board configuration, white should have 20 possible moves
            board.GetMoves().Should().HaveCount(20);

            // nothing yet in attack range
            board.GetAttackMoves().Should().HaveCount(0);

            // can also search all possible moves based on the current board configuration, ignoring which players turn it is
            //board.GetAllMovesForAllPieces().Should().HaveCount(40);
        }

        [Fact]
        public void ShouldAllowMakingMoves()
        {
            var board = new Board();
            // no problem here since GetMoves should only return valid moves anyway
            var move = board.GetMoves().First();

            board[move.from].Should().Be(move.piece);

            board.Move(move);

            board[move.to].Should().Be(move.piece);
        }

        [Fact]
        public void ShouldNotPreventMakingInvalidMoves()
        {
            var board = new Board();
            // In the interest of performance, move validation happens only once (inside Board.GetMoves())
            // therefore, creating moves manually is not reccommended
            // you should use the moves returned by GetMoves() instead.

            var piece = board[6, 2]; // get a white pawn
            var move = new Move(piece, new Square(2, 6), new Square(4, 4)); // create an invalid move for a pawn

            board[move.from].Should().Be(move.piece);

            board.Move(move);

            // board.Move allows the invalid move
            board[move.to].Should().Be(move.piece);
        }

        [Fact]
        public void ShouldCheckIfMoveEvadesCheck()
        {
            var board = new Board(false);
            var wKing = board.AddPiece(PieceType.King, Player.White, new Square(4, 4));
            var bRook = board.AddPiece(PieceType.Rook, Player.Black, new Square(4, 0)); // white king in check by black rook
            var wRook = board.AddPiece(PieceType.Rook, Player.White, new Square(0, 0)); // black rook at risk by white rook
            var bKing = board.AddPiece(PieceType.King, Player.Black, new Square(7, 0)); // black king is currently safe
            board.Start();

            board.MoveEvadesCheck(new Move(wKing, wKing.location, new Square(4, 3))).Should().BeFalse();
            board.MoveEvadesCheck(new Move(wKing, wKing.location, new Square(3, 4))).Should().BeTrue();
            board.MoveEvadesCheck(new Move(wKing, wKing.location, new Square(5, 4))).Should().BeTrue();
            board.MoveEvadesCheck(new Move(wKing, wKing.location, new Square(4, 5))).Should().BeFalse();
        }

        [Fact]
        public void ShouldBeStartedWhenBoardIsNotInitialized()
        {
            var board = new Board(false);
            var wKing = board.AddPiece(PieceType.King, Player.White, new Square(4, 4));
            var bKing = board.AddPiece(PieceType.King, Player.Black, new Square(7, 0));

            // because we passed false to the board constructor
            // the board is not yet in a valid play state
            board.ply.Should().Be(0);
            Player.None.Should().Be(board.CurrentPlayer);

            // we can explicitly call start to finish initialization
            board.Start();
            board.ply.Should().Be(1);
            Player.White.Should().Be(board.CurrentPlayer);
        }

        [Fact]
        public void ShouldFilterMovesWhenPlayerIsInCheck()
        {
            var board = new Board(false);
            var wKing = board.AddPiece(PieceType.King, Player.White, new Square(4, 4));
            var bRook = board.AddPiece(PieceType.Rook, Player.Black, new Square(4, 0)); // white king in check by black rook
            var wRook = board.AddPiece(PieceType.Rook, Player.White, new Square(0, 0)); // black rook at risk by white rook
            var bKing = board.AddPiece(PieceType.King, Player.Black, new Square(7, 0)); // black king is currently safe
            board.Start();

            // peek at black moves to assert that the white king is in check
            //board.GetChecksFromBlack().Any().Should().BeTrue();
            //var blackMoves = board.GetChecksFromBlack().ToList();
            //blackMoves.Should().HaveCountGreaterThan(0);
            //blackMoves.Where(m => m.piece == bRook).Select(m => m.to).Should().Contain(wKing.location);

            // assert the white is the current player and is in check
            (board.CurrentPlayer == Player.White).Should().BeTrue();
            Player.White.Should().Be(board.CurrentPlayer);
            board.CurrentPlayerInCheck.Should().BeTrue();

            // get valid moves
            var moves = board.GetMoves();

            // verify some moves that shouldn't be allowed since the white king would still be in check
            var wKingMoves = moves.Where(x => x.piece == wKing).Select(m=>m.to).ToList();
            wKingMoves.Should().NotContain(new Square(4, 3));
            wKingMoves.Should().NotContain(new Square(4, 5));

            // verify moves by other pieces that remove the white king from check are allowed
            var wRookMoves = moves.Where(x => x.piece == wRook).Select(m => m.to).ToList();
            wRookMoves.Should().Contain(new Square(4, 0));
        }

        [Fact]
        public void ShouldSaveToFenString()
        {
            var board = new Board();
            var fen = board.ToString();
            fen.Should().Be("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w 1");
        }

        [Fact]
        public void ShouldLoadFromFenString()
        {
            var board = new Board("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w 1");
            var fen = board.ToString();
            fen.Should().Be("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w 1");
            Player.White.Should().Be(board.CurrentPlayer);
            board.ply.Should().Be(1);
        }

        [Theory]
        [InlineData("4k3/3PP3/4K3/8/8/8/8/8 b 2")]
        [InlineData("R5k1/5ppp/8/8/8/8/8/6K1 b 2")]
        [InlineData("6k1/5pQp/8/8/8/8/8/B5K1 b 2")]
        public void CheckMate(string fen)
        {
            var board = new Board(fen);
            board.IsMate.Should().BeTrue();
        }
    }
}
