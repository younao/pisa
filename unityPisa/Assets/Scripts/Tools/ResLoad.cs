using UnityEngine;
using UnityEditor;

public class ResLoad
{
    private static ResLoad _self;
    public static ResLoad self
    {
        get
        {
            if (_self == null)
            {
                _self = new ResLoad();

            }
            return _self;
        }
    }

    
}