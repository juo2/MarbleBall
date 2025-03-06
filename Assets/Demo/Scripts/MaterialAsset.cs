using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MaterialAsset", menuName = "Assets/Material Asset", order = 1)]
public class MaterialAsset : ScriptableObject
{
    // 用来存储多个材质的列表
    public List<Material> materials;
}
