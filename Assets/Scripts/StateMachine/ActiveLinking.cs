using System;
using System.Collections.Generic;
using Scripts.Characters;



public class ActiveLinking
{
    public IEnumerable<EventStateLinking> links;
    public BaseState state;
    public Dictionary<string, System.Object> linkingProperties;
    public float timeStarted;

    public T GetValueOrDefault<T>(string key, T @default = default(T))
    {
        System.Object value;
        if (linkingProperties.TryGetValue(key, out value))
        {
            if (value is T)
            {
                return (T)value;
            }
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (InvalidCastException)
            {
                return @default;
            }
        }
        return @default;
    }



}


