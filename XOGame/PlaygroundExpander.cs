using System;
using System.Collections.Generic;
using AITickTackToe.AI;

namespace AITickTackToe.TickTackToeGame
{
    /// <summary>
    /// Expands TickToeGame
    /// </summary>
    public class PlaygroundExpander : IDecisionNodeExpander<Playground>
    {
        private char _myChar = 'x';

        public char MyChar
        {
            get => _myChar;
            init
            {
                _myChar = value;
                _opponentChar = MyChar == 'x' ? 'o' : 'x';
            }
        }
        private char _opponentChar = 'o';
        public Memory<Playground> Expand(Playground pg, DecisionNodeType type)
        {
            char currentTurn = type == DecisionNodeType.And ? _myChar : _opponentChar;
            var newStates = new List<Playground>();
            if (pg.InMovingState(currentTurn))
            {
                int nr, nc;
                for (int r = 0; r < Playground.Length; r++)
                {
                    for (int c = 0; c < Playground.Length; c++)
                    {
                        if (pg[r, c] != currentTurn) { continue; }
                        for (int m = 0; m < Playground.Movements.Length; m++)
                        {
                            nr = r + Playground.Movements[m][0];
                            nc = c + Playground.Movements[m][1];
                            if (nr < 0 || nr >= Playground.Length || nc < 0 || nc >= Playground.Length || pg[nr, nc] != Playground.Empty) { continue; }
                            newStates.Add(pg.Move(r, c, nr, nc));
                        }
                    }
                }
            }
            else
            {
                for (int r = 0; r < Playground.Length; r++)
                {
                    for (int c = 0; c < Playground.Length; c++)
                    {
                        if (pg[r, c] != Playground.Empty) { continue; }
                        newStates.Add(pg.Set(r, c, currentTurn));
                    }
                }
            }
            return newStates.ToArray();
        }
    }
}