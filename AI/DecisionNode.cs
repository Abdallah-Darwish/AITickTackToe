using System;
using System.Collections.Generic;
using System.Linq;
namespace AITickTackToe.AI
{
    public enum DecisionNodeType
    {
        Or, And
    }
    //RENAME ME FFS
    public class DecisionNodeDecision : IComparable<DecisionNodeDecision>, IEquatable<DecisionNodeDecision>
    {
        public EvaluationResult Value { get; init; }
        public int Distance { get; init; } = 0;

        public int CompareTo(DecisionNodeDecision? other)
        {
            if (other == null) { return 1; }
            if (other.Value.Equals(Value)) { return Distance.CompareTo(other.Distance); }
            return Value.CompareTo(other.Value);
        }

        public bool Equals(DecisionNodeDecision? other)
        {
            return other != null && Value.Equals(other.Value) && Distance == other.Distance;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Distance);
        }
        public override bool Equals(object? obj) => Equals(obj as DecisionNodeDecision);
        public override string ToString() => $"{{Value: {Value}, Distance: {Distance}}}";

        public static bool operator <(DecisionNodeDecision a, DecisionNodeDecision b) => a.CompareTo(b) < 0;
        public static bool operator >(DecisionNodeDecision a, DecisionNodeDecision b) => a.CompareTo(b) > 0;

        public static bool operator <=(DecisionNodeDecision a, DecisionNodeDecision b) => a.CompareTo(b) <= 0;
        public static bool operator >=(DecisionNodeDecision a, DecisionNodeDecision b) => a.CompareTo(b) >= 0;

    }
    public class DecisionNode<T>
    {
        public bool IsSelected { get; set; }
        public bool IsBest { get; set; }
        public DecisionNodeType Type { get; }
        public T Value { get; }
        public EvaluationResult Weight { get; }
        public DecisionNode<T>? Parent { get; private set; }
        ///<summary>My best descendant weight and distance (or number) of moves to it.</summary>
        public DecisionNodeDecision Decision { get; private set; }
        public ReadOnlyMemory<DecisionNode<T>> Descendants { get; private set; } = new ReadOnlyMemory<DecisionNode<T>>();
        public void Expand(IDecisionNodeExpander<T> ex, IDecisionNodeEvaluator<T> ev, int levels = 1)
        {
            if (Weight.IsInf || levels <= 0) { return; }

            //TEST Case: Expansion level is 7, fill upper right and lower left and mid left, AI should take upper left diagonal and win
            //TODO FIX MEEEEEEEEEEEEE
            //WE HAVE SOME Mind fuck here,
            //If I am an "or" node then I want best for me and closest, or worst and furthest
            //If I am "and" node then I want worst and closest, or best and furthest
            var des = new Dictionary<EvaluationResult, DecisionNode<T>>();
            var newStatesType = Type == DecisionNodeType.And ? DecisionNodeType.Or : DecisionNodeType.And;
            var newStates = ex.Expand(Value, newStatesType);

            foreach (var s in newStates.Span)
            {
                var w = ev.Evaluate(s);
                var stateNode = new DecisionNode<T>(this, s, w);
                stateNode.Expand(ex, ev, levels - 1);
                if (!des.TryGetValue(stateNode.Decision.Value, out var d))
                {
                    des.Add(stateNode.Decision.Value, stateNode);
                }
                else if (Type == DecisionNodeType.Or)
                {
                    if (d.Decision.Value.Value >= 0)
                    {
                        if (stateNode.Decision.Distance < d.Decision.Distance)
                        {
                            des[stateNode.Decision.Value] = stateNode;
                        }
                    }
                    else
                    {
                        if (stateNode.Decision.Distance > d.Decision.Distance)
                        {
                            des[stateNode.Decision.Value] = stateNode;
                        }
                    }
                }
                else
                {
                    if (d.Decision.Value.Value < 0)
                    {
                        if (stateNode.Decision.Distance < d.Decision.Distance)
                        {
                            des[stateNode.Decision.Value] = stateNode;
                        }
                    }
                    else
                    {
                        if (stateNode.Decision.Distance > d.Decision.Distance)
                        {
                            des[stateNode.Decision.Value] = stateNode;
                        }
                    }
                }
            }
            Descendants = des.Values.ToArray();

            var bestDes = Descendants.Span[0];

            if (Type == DecisionNodeType.And)
            {
                foreach (var d in Descendants.Span)
                {
                    d.IsBest = false;
                    if (d.Decision < bestDes.Decision)
                    {
                        bestDes = d;
                    }
                }
            }
            else
            {
                foreach (var d in Descendants.Span)
                {
                    d.IsBest = false;
                    if (d.Decision > bestDes.Decision)
                    {
                        bestDes = d;
                    }
                }
            }
            Decision = new DecisionNodeDecision
            {
                Distance = bestDes.Decision.Distance + 1,
                Value = bestDes.Decision.Value
            };
            bestDes.IsBest = true;
        }
        private DecisionNode(DecisionNode<T>? parent, T value, EvaluationResult weight)
        {
            Parent = parent;
            Type = parent == null ? DecisionNodeType.Or : (parent.Type == DecisionNodeType.And ? DecisionNodeType.Or : DecisionNodeType.And);
            Value = value;
            Weight = weight;
            Decision = new DecisionNodeDecision { Value = Weight };
        }
        public DecisionNode(T value, EvaluationResult weight, DecisionNodeType type = DecisionNodeType.Or) : this(null, value, weight)
        {
            Type = type;
        }

        public DecisionNode<T> BestSon
        {
            get
            {
                var des = Descendants.Span;
                for (int i = 0; i < des.Length; i++)
                {
                    if (des[i].IsBest) { return des[i]; }
                }
                return null!;
            }
        }
    }
}