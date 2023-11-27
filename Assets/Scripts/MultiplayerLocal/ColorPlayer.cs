using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPlayer : MonoBehaviour
{
    public MeshRenderer MeshKayak;
    public List<Material> MaterialColorKayak;


    public List<SkinnedMeshRenderer> MeshCharacter;
    public List<Material> MaterialColorCharacter;


    private int _indexColor;


    public void ChangeColor()
    {
        _indexColor = (_indexColor + 1) % MaterialColorKayak.Count;

        foreach (var item in MeshCharacter)
        {
            item.material = MaterialColorCharacter[_indexColor];
        }

        Debug.Log(MaterialColorCharacter[_indexColor].name);
        MeshKayak.material = MaterialColorKayak[_indexColor];
    }
    public void InitColor(int index)
    {
        _indexColor = (index) % MaterialColorKayak.Count;

        foreach (var item in MeshCharacter)
        {
            item.material = MaterialColorCharacter[index];
        }

        Debug.Log(MaterialColorCharacter[index].name);
        MeshKayak.material = MaterialColorKayak[index];
    }

}
