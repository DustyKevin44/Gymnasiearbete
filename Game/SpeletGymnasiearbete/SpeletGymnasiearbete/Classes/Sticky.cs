using System;


namespace SpeletGymnasiearbete.Classes;

// A Type that sticks to another Type
public class Sticky<T>(Func<T> getBaseValue, T offset, Func<T, T, T> combine)
{
    private readonly Func<T> _getBaseValue = getBaseValue; // Lambda to get the value to stick to
    private T _offset = offset;
    private readonly Func<T, T, T> _combine = combine; // How to combine the base value and the offset

    // Sticky value
    public T Value {
        get { return _combine(_getBaseValue(), _offset); }
        set { _offset = value; }
    }
}