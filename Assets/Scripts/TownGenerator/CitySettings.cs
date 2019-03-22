
using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CitySettings", menuName = "CityGenerator/City")]
public class CitySettings : ScriptableObject
{
    public string cityName;
    public int patchNum;
    public int seed;
}