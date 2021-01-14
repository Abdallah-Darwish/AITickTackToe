using System;
using System.Collections.Generic;

namespace AITickTackToe.AI
{
    /// <summary>
    /// Conatiner for evaluation result of <see cref="DecisionNode{T}"/> value.
    /// </summary>
    public class EvaluationResult : IComparable<EvaluationResult>, IEquatable<EvaluationResult>
    {
        public const int INF = 1000_000_000;
        public bool IsInf => Value == INF || Value == -INF;
        /// <summary>
        /// Until now(and I don't think I will edit the project) its used for debugging purposes.
        /// </summary>
        public string? Comment { get; init; }
        //EXTRA VERBOSE
        /// <summary>
        /// Weight or evaluation process result
        /// </summary>
        public int Value { get; init; }
        public int CompareTo(EvaluationResult? other)
        {
            if (other == null) { return 1; }
            return Value == other.Value ? 0 : (Value < other.Value ? -1 : 1);
        }
        /// <summary>
        /// Text representation of <see cref="Value"/> where if <see cref="IsInf"/> is true then this will return infinty symbol.
        /// </summary>
        public string PrintableValue => Value switch
        {
            INF => "\u221E",
            -INF => "-\u221E",
            _ => Value.ToString()
        };
        public bool Equals(EvaluationResult? other) => Value == other?.Value;
        public override bool Equals(object? obj) => Equals(obj as EvaluationResult);
        public override int GetHashCode() => Value;

        public override string ToString() => $"{Value}{(Comment == null ? "" : $": {Comment}")}";
        public static bool operator <(EvaluationResult? e1, EvaluationResult? e2) => (e1?.CompareTo(e2) ?? -1) < 0;
        public static bool operator >(EvaluationResult? e1, EvaluationResult? e2) => (e1?.CompareTo(e2) ?? -1) > 0;
    }
    /// <summary>
    /// Evaluates the value of <see cref="DecisionNode{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of <see cref="DecisionNode{T}"/> value</typeparam>
    public interface IDecisionNodeEvaluator<T>
    {
        EvaluationResult Evaluate(T val);
    }
}