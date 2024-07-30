using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MonoGame.Extended;

namespace Farkle.Rules.DiceTypes;

public abstract class DiceBase
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public int Value { get; set; }

    private (float Weight, float CumulativeWeight)[] _weights = new (float, float)[6];
    public (float Weight, float CumulativeWeight)[] Weights => _weights;
    private float _cumulativeWeight;

    protected void SetWeights(float side1, float side2, float side3, float side4, float side5, float side6)
    {
        SetWeights([side1, side2, side3, side4, side5, side6]);
    }
        
    private void SetWeights(float[] weights)
    {
        if (weights.Length != _weights.Length)
            throw new ArgumentException($"Array must contain {_weights.Length} values.", nameof(weights));

        _cumulativeWeight = 0f;
        for (var i = 0; i < weights.Length; i++)
        {
            _cumulativeWeight += weights[i];
            _weights[i] = (Weight: weights[i], CumulativeWeight: _cumulativeWeight);
        }
    }

    public virtual void Roll()
    {
        var r = GameMain.Random.NextSingle() * _cumulativeWeight;
        for (var i = 0; i < _weights.Length; i++)
        {
            if (r < _weights[i].CumulativeWeight) 
            {
                Value = i + 1;
                return;
            }
        }

        throw new Exception("Failed to determine side from weighted values.");
    }

    private sealed class DiceBaseEqualityComparer : IEqualityComparer<DiceBase>
    {
        public bool Equals(DiceBase x, DiceBase y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name && x.Description == y.Description && x.Value == y.Value && Equals(x._weights, y._weights) && x._cumulativeWeight.Equals(y._cumulativeWeight);
        }

        public int GetHashCode(DiceBase obj)
        {
            return HashCode.Combine(obj.Name, obj.Description, obj.Value, obj._weights, obj._cumulativeWeight);
        }
    }

    public static IEqualityComparer<DiceBase> DiceBaseComparer { get; } = new DiceBaseEqualityComparer();
}