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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeColor();
        }
    }

    public void ChangeColor()
    {



        _indexColor = (_indexColor + 1) % MaterialColorKayak.Count;

        foreach (var item in MeshCharacter)
        {
            item.material = MaterialColorCharacter[_indexColor];
        }
        MeshKayak.material = MaterialColorKayak[_indexColor];
    }

}
