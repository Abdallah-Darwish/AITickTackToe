using System;
using System.Collections.Generic;

namespace AITickTackToe.AI
{
    public interface IDecisionNodeExpander<T>
    {
        ///<summary>Returns a list of all possible moves from the current value.</summary>
        ///<param name="type">Type of the new <see cref="DecisionNode{T}"/>s to generate.</param>
        Memory<T> Expand(T currentVal, DecisionNodeType type);
    }
}