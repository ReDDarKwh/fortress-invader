using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Missions;


[CreateAssetMenu(fileName = "FortressInfo", menuName = "Util/FortressInfo")]
public class FortressInfo : ScriptableObject
{
    public Mission currentMission;
    public IEnumerable<Mission> missions;
}
