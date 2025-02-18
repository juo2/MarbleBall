using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererDemo : MonoBehaviour
{
    [SerializeField] public LineRenderer _line;
    private static Material s_Material;
    private static Dictionary<Color, Material> s_MaterialColorDic = new Dictionary<Color, Material>();

    public void AddLineRenderer(Vector3 pos)
    {
        _line.positionCount = _line.positionCount + 1;
        _line.SetPosition(_line.positionCount - 1, pos);
    }

    public void ClearLineRenderer()
    {
        _line.positionCount = 0;
    }

    public void ResetLineRenderer(List<PhysicsState> physicsStates)
    {
        _line.positionCount = physicsStates.Count;

        for(int i = 0;i < physicsStates.Count;i++)
        {
            _line.SetPosition(i, physicsStates[i].position);
        }
    }

    public void SetColor(Color color)
    {
        _line.material = GetMaterial(color);
    }

    static Material GetMaterial(Color color)
    {
        if(s_Material == null)
            s_Material = Resources.Load<Material>("LINE");

        Material mat = null;

        if (!s_MaterialColorDic.ContainsKey(color))
        {
            mat = Instantiate(s_Material);
            mat.color = color;
            s_MaterialColorDic.Add(color, mat);
        }
        else
        {
            mat = s_MaterialColorDic[color];
        }

        return mat;
    }

    public void SetWidth(float width)
    {
        //_line.startWidth = width;
        //_line.endWidth = width;
        _line.widthMultiplier = width;
    }
}
