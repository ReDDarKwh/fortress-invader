
using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CitySettings", menuName = "CityGenerator/City")]
public class CitySettings : ScriptableObject
{
    public int patchNum;
    public int seed;
    public bool wall;
    public bool plaza;
}