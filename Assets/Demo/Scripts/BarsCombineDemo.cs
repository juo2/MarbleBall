using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsCombineDemo : BarsDemo
{
    public BarsScaleDemo barsScaleDemo;

    public PadCollider padCollider1;
    public PadCollider padCollider2;
    public PadCollider padCollider3;

    public bool isPass1(Vector3 framePos,Vector3 frameDir)
    {
        if(frameDir.x > 0)
        {
            return framePos.x > barsScaleDemo.scaler.transform.position.x;
        }
        else
        {
            return framePos.x < barsScaleDemo.scaler.transform.position.x;
        }
    }

    public bool isPass2(Vector3 framePos)
    {
        return framePos.y < endPoint.transform.position.y;
    }

    public override void updateVirtualBars(bool res)
    {
        barsScaleDemo.updateVirtualBars(res);
    }

    public void updateBar(Vector3 bornPos, Vector3 framePos)
    {
        barsScaleDemo.updateBar(bornPos,framePos);
    }

    public float getScaleX()
    {
        return barsScaleDemo.getScaleX();
    }

    public void SetScaleX(float scaleX)
    {
        barsScaleDemo.SetScaleX(scaleX);
    }
}
