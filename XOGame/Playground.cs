using System;
using System.Collections.Generic;

namespace AITickTackToe
{
    public class Playground
    {
        public static readonly int[][] Movements = new int[4][]
        {
            new int[2] { -1, 0 },
            new int[2] { 1, 0 },
            new int[2] { 0, -1 },
            new int[2] { 0, 1 },
        };
        public const int Length = 3;
        public const char Empty = default;
        public int SetX { get; private set; }
        public int SetO { get; private set; }

        public int Count(char c)
        {
            if (c == 'x') { return SetX; }
            if (c == 'o') { return SetO; }
            return _cells.Length - SetX - SetO;
        }
        private bool _calcedWinner = false;
        private char _winner;
        private ((int Row, int Col), (int Row, int Col)) _winningLine;
        public char Winner
        {
            get
            {
                CalcWinner();
                return _winner;
            }
        }

        public ((int Row, int Col) Start, (int Row, int Col) End) WinningLine
        {
            get
            {
                CalcWinner();
                return _winningLine;
            }
        }
        private void CalcWinner()
        {
            if (_calcedWinner) { return; }
            _calcedWinner = true;
            _winner = Empty;
            if (SetX < 3 && SetO < 3) { return; }
            char z;
            bool f;
            //rows
            for (int r = 0; r < Length; r++)
            {
                z = this[r, 0];
                if (z == Empty) { continue; }
                f = true;
                for (int c = 1; c < Length; c++)
                {
                    f &= this[r, c] == z;
                }
                if (f)
                {
                    _winner = z;
                    _winningLine = ((r, 0), (r, 2));
                    return;
                }
            }

            //cols
            for (int c = 0; c < Length; c++)
            {
                z = this[0, c];
                if (z == Empty) { continue; }
                f = true;

                for (int r = 1; r < Length; r++)
                {
                    f &= this[r, c] == z;
                }
                if (f)
                {
                    _winner = z;
                    _winningLine = ((0, c), (2, c));
                    return;
                }
            }

            //diag 1
            f = true;
            z = this[0, 0];
            if (z != Empty)
            {
                for (int r = 1, c = 1; r < Length; r++)
                {
                    f &= this[r, c] == z;
                    c++;
                }
                if (f)
                {
                    _winner = z;
                    _winningLine = ((0, 0), (2, 2));
                    return;
                }
            }

            //diag 2
            f = true;
            z = this[0, 2];
            if (z != Empty)
            {
                for (int r = 1, c = 1; r < Length; r++)
                {
                    f &= this[r, c] == z;
                    c--;
                }
                if (f)
                {
                    _winner = z;
                    _winningLine = ((0, 2), (2, 0));
                }
            }
        }
        public bool InMovingState(char c)
        {
            if (c == 'x') { return SetX >= 3; }
            if (c == 'o') { return SetO >= 3; }
            return false;
        }
        private readonly char[] _cells = new char[9];
        public ReadOnlyMemory<char> Cells => new ReadOnlyMemory<char>(_cells);
        private ref char C(int row, int col) => ref _cells[row * Length + col];
        public char this[int row, int col]
        {
            get { return _cells[row * Length + col]; }
            private set
            {
                ref char c = ref C(row, col);
                if (c == value) { return; }
                if (c == 'x') { SetX--; }
                else if (c == 'o') { SetO--; }

                if (value == 'x') { SetX++; }
                else if (value == 'o') { SetO++; }
                c = value;
            }
        }
        public Playground Set(int r, int c, char v)
        {
            var res = Clone();
            res[r, c] = v;
            return res;
        }
        public Playground Move(int cr, int cc, int nr, int nc)
        {
            /*
            Faster to let the array indexer check only
            if (cr < 0 || cc < 0 || nr < 0 || nc < 0)
            {
                throw new ArgumentOutOfRangeException("", "One of the arguments is negative.");
            }
            if (cr >= Row || cr >= Row || nr >= Row || nc >= Row)
            {
                throw new ArgumentOutOfRangeException("", $"One of the arguments is >= {Row}.");
            }
            */

            if ((Math.Abs(nr - cr) + Math.Abs(nc - cc)) > 1)
            {
                throw new ArgumentException("The new cell is not neighbour to the old cell.");
            }
            if (C(nr, nc) != Empty)
            {
                throw new InvalidOperationException($"Cell(row: {nr}, col: {nc}) is not empty.");
            }
            var res = Clone();
            res[nr, nc] = res[cr, cc];
            res[cr, cc] = Empty;
            return res;
        }
        public Playground Clone()
        {
            var clone = new Playground
            {
                SetX = SetX,
                SetO = SetO
            };
            _cells.CopyTo(clone._cells, 0);
            return clone;
        }
    }
}