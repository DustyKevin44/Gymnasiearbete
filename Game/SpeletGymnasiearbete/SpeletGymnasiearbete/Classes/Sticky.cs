using System;


namespace SpeletGymnasiearbete.Classes;

public class Sticky<T>(Func<T> getBaseValue, T offset, Func<T, T, T> combine)
{
    private readonly Func<T> _getBaseValue = getBaseValue;
    private T _offset = offset;
    private readonly Func<T, T, T> _combine = combine;

    public T Value {
        get { return _combine(_getBaseValue(), _offset); }
        set { _offset = value; }
    }
}