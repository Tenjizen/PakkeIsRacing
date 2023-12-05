using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlayer : MonoBehaviour
{
    public CharacterMultiPlayerManager Character;


    public MeshRenderer MeshKayak;
    public List<ColorCharacter> Colors = new List<ColorCharacter>();

    public List<SkinnedMeshRenderer> MeshCharacter = new List<SkinnedMeshRenderer>();

    public ColorCharacter CurrentColor;
    
    private int _indexColor;

    private void Start()
    {
        CurrentColor = Colors[0];
        Character.ChangeColorParticule();
    }

    public void ChangeColor()
    {
        _indexColor = (_indexColor + 1) % Colors.Count;

        foreach (var item in MeshCharacter)
        {
            item.material = Colors[_indexColor].MaterialColorCharacter;
        }

        MeshKayak.material = Colors[_indexColor].MaterialColorKayak;

        CurrentColor = Colors[_indexColor];
        Character.ChangeColorParticule();
    }
    public void InitColor(int index)
    {
        _indexColor = (index) % Colors.Count;

        foreach (var item in MeshCharacter)
        {
            item.material =  Colors[_indexColor].MaterialColorCharacter;
        }

        MeshKayak.material =  Colors[_indexColor].MaterialColorKayak;
    }

}

[Serializable]
public class ColorCharacter
{
    public Colors Color;
    public Material MaterialColorKayak;
    public Material MaterialColorCharacter;
    public ParticleSystem PrefabRespawnFX;
}

public enum Colors
{
    White,
    Red,
    Green,
    Blue,
    Purple,
    Yellow
}