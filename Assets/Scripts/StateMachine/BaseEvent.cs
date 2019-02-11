using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.Characters;


[System.Serializable]
public enum EventCheckerType
{
    CHECK_ONCE,
    CHECK_CONTINUOUSLY
}

public abstract class BaseEvent : ScriptableObject
{

    public EventCheckerType checkerType = EventCheckerType.CHECK_CONTINUOUSLY;
    public abstract bool Check(GameObject gameObject, ActiveLinking activeLinking, out EventMessage message);

    private void OnEnable()
    {
        // this.continuousCherckers = checkers
        // .Where(x => x.checkerType == EventCheckerType.CHECK_CONTINUOUSLY).ToList();
        // this.onceCherckers = checkers
        // .Where(x => x.checkerType == EventCheckerType.CHECK_ONCE).ToList();
    }

    public abstract void Init(ActiveLinking activeLinking);


}

