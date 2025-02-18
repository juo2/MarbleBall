using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsScaleDemo : MonoBehaviour
{
    public Transform wrapper;
    public Transform scaler;
    public Transform virtualScaler;
    public Transform endPoint;
    public float beiginOffset = 0.15f;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void updateVirtualBars(bool res)
    {
        //scaler.localScale = new Vector3(1000f, 1f, 1f);
        virtualScaler.gameObject.SetActive(res);
    }

    public void updateBar(Vector3 bornPos,Vector3 framePos)
    {
        float originLength = endPoint.position.x - scaler.position.x;
        scaler.localScale = new Vector3( (framePos.x - bornPos.x) / originLength, 1f , 1f );
    }

    public float getScaleX()
    {
        return scaler.localScale.x;
    }

    public void SetScaleX(float scaleX)
    {
        scaler.localScale = new Vector3(scaleX, 1f, 1f);
    }

    public bool isPass2(Vector3 framePos)
    {
        return framePos.y < endPoint.transform.position.y;
    }

    public bool isPassXY(Vector3 framePos)
    {
        if (transform.eulerAngles.y == 0 && framePos.x - endPoint.position.x > 0)
        {
            if (framePos.y < endPoint.position.y)
            {
                if (Mathf.Abs(framePos.y - endPoint.position.y) > 0.1f)
                {
                    return true;
                }
            }

            if (Mathf.Abs(framePos.x - endPoint.position.x) > 0.1f)
            {
                return true;
            }
        }
        else if (transform.eulerAngles.y == 180 && framePos.x - endPoint.position.x < 0)
        {
            if (framePos.y < endPoint.position.y)
            {
                if (Mathf.Abs(framePos.y - endPoint.position.y) > 0.1f)
                {
                    return true;
                }
            }

            if (Mathf.Abs(framePos.x - endPoint.position.x) > 0.1f)
            {
                return true;
            }
        }

        return false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
