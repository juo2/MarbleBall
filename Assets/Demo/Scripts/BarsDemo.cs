using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarsDemo : MonoBehaviour
{
    public Transform endPoint;
    public float beiginOffset = 0.3f;
    public PadCollider padCollider;
    //[SerializeField]
    //bool isPositive = true;
    public bool inverse = false;
    public int mistakeCollisionTimes = 0;

    public int beginFrame = -1;
    public int endFrame = -1;

    public bool followRotation = false;

    public int prefabIndex = -1;

    public bool inverse_vec = false;

    List<SplineMesh.Channel> channelList = new List<SplineMesh.Channel>();
    List<Vector3> originRotation = new List<Vector3>();

    public List<SplineComputer> splineComputerList = new List<SplineComputer>();

    public List<GameObject> skinList = new List<GameObject>();
    public bool isNeedTurning = true;
    private void Awake()
    {
        //int n = 0;
        //SampleCollection sampleCollection = new SampleCollection();
        //splineComputerVirtual.GetSamples(sampleCollection);

        //SplineSample[] splineSamples = sampleCollection.samples;
        //int _length = splineSamples.Length;
        //List<SplinePoint> splinePointList = new List<SplinePoint>();

        //for (int i = _length - 1; i >= n; i--)
        //{
        //    SplineSample ss = splineSamples[i];
        //    SplinePoint splinePoint = new SplinePoint();
        //    splinePoint.position = transform.localToWorldMatrix.MultiplyPoint(ss.position);
        //    splinePoint.size = 1;
        //    splinePoint.normal = new Vector3(0, 0, 1);
        //    splinePointList.Add(splinePoint);

        //    if (i == n)
        //        endPoint.transform.position = new Vector3(splinePoint.position.x, splinePoint.position.y, splinePoint.position.z);
        //}

        //splineComputer.SetPoints(splinePointList.ToArray());
        //splineComputer.ResampleTransform();

        //SplineMesh.Channel channel = splineMesh.GetChannel(2);
        //SplineMesh.Channel.MeshDefinition mesh = channel.GetMesh(0);
        //mesh.offset = new Vector3(0.03f, 0, 0);
        //mesh.rotation = new Vector3(0, -90, 90);
        if(isNeedTurning)
        {
            if (splineComputerList.Count == 0)
            {
                SplineMesh[] splineMeshes = GetComponentsInChildren<SplineMesh>(true);
                foreach (var sm in splineMeshes)
                {
                    for (int i = 0; i < sm.GetChannelCount(); i++)
                    {
                        Dreamteck.Splines.SplineMesh.Channel channel = sm.GetChannel(i);
                        if (channel.name == "Channel 3")
                        {
                            channelList.Add(channel);
                            originRotation.Add(channel.minRotation);
                        }
                    }
                }
            }
            else
            {
                foreach (var sc in splineComputerList)
                {
                    SplineMesh[] splineMeshes = sc.transform.GetComponentsInChildren<SplineMesh>(true);
                    foreach (var sm in splineMeshes)
                    {
                        for (int i = 0; i < sm.GetChannelCount(); i++)
                        {
                            Dreamteck.Splines.SplineMesh.Channel channel = sm.GetChannel(i);
                            if (channel.name == "Channel 3")
                            {
                                channelList.Add(channel);
                                originRotation.Add(channel.minRotation);
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void refreshSkin(int skin)
    {
        foreach(var skinObj in skinList)
        {
            skinObj.SetActive(false);
        }


        if (skin < skinList.Count)
            skinList[skin].SetActive(true);
        else
            Debug.LogError($"{this.gameObject.name} ²»´æÔÚ skin:{skin}");
    }

    public void refreshBarDir()
    {
        if (inverse)
        {
            for(int i = 0;i < channelList.Count;i++)
            {
                Vector3 rotation = originRotation[i];
                channelList[i].maxRotation = new Vector3(rotation.x + 180f, rotation.y, rotation.z);
                channelList[i].minRotation = new Vector3(rotation.x + 180f, rotation.y, rotation.z);
            }
        }
        else
        {
            for (int i = 0; i < channelList.Count; i++)
            {
                Vector3 rotation = originRotation[i];
                channelList[i].maxRotation = new Vector3(rotation.x, rotation.y, rotation.z);
                channelList[i].minRotation = new Vector3(rotation.x, rotation.y, rotation.z);
            }
        }
    }

    public void RefreshMesh(bool res)
    {
        //SplineMesh.Channel channel = splineMesh.GetChannel(2);
        //SplineMesh.Channel.MeshDefinition mesh = channel.GetMesh(0);

        //if(res)
        //{
        //    mesh.offset = new Vector3(0.03f, 0, 0);
        //    mesh.rotation = new Vector3(0, -90, 90);
        //}
        //else
        //{
        //    mesh.offset = new Vector3(-0.03f, 0, 0);
        //    mesh.rotation = new Vector3(0, 90, 90);
        //}
    }

    public virtual void updateVirtualBars(bool res)
    {
        //if(splineComputerVirtual != null)
        //    splineComputerVirtual.gameObject.SetActive(res);

        //if(splineComputer != null)
        //    splineComputer.gameObject.SetActive(!res);
    }

    public bool isPass(Vector3 framePos)
    {
        return framePos.y < endPoint.position.y;
    }

    public bool isPassX(Vector3 framePos)
    {
        if(inverse)
        {
            return framePos.x > endPoint.position.x;
        }
        else
        {
            return framePos.x < endPoint.position.x;
        }
    }

    public bool isPassXY(Vector3 framePos,float endOffset)
    {
        if(inverse_vec)
        {
            if (!inverse && framePos.x - endPoint.position.x > 0)
            {
                if (framePos.y < endPoint.position.y)
                {
                    if (Mathf.Abs(framePos.y - endPoint.position.y) > endOffset)
                    {
                        return true;
                    }
                }

                if (Mathf.Abs(framePos.x - endPoint.position.x) > endOffset)
                {
                    return true;
                }
            }
            else if (inverse && framePos.x - endPoint.position.x < 0)
            {
                if (framePos.y < endPoint.position.y)
                {
                    if (Mathf.Abs(framePos.y - endPoint.position.y) > endOffset)
                    {
                        return true;
                    }
                }

                if (Mathf.Abs(framePos.x - endPoint.position.x) > endOffset)
                {
                    return true;
                }
            }
        }
        else
        {
            if (!inverse && framePos.x - endPoint.position.x < 0)
            {
                if (framePos.y < endPoint.position.y)
                {
                    if (Mathf.Abs(framePos.y - endPoint.position.y) > endOffset)
                    {
                        return true;
                    }
                }

                if (Mathf.Abs(framePos.x - endPoint.position.x) > endOffset)
                {
                    return true;
                }
            }
            else if (inverse && framePos.x - endPoint.position.x > 0)
            {
                if (framePos.y < endPoint.position.y)
                {
                    if (Mathf.Abs(framePos.y - endPoint.position.y) > endOffset)
                    {
                        return true;
                    }
                }

                if (Mathf.Abs(framePos.x - endPoint.position.x) > endOffset)
                {
                    return true;
                }
            }
        }
        

        return false;
    }

    public int SplineSampleIndex(Vector3 framePos)
    {
        //SampleCollection sampleCollection = new SampleCollection();
        //splineComputerVirtual.GetSamples(sampleCollection);

        //SplineSample[] splineSamples = sampleCollection.samples;

        int minIndex = int.MaxValue;
        //float minDistance = float.MaxValue;

        //for(int i = 0;i < splineSamples.Length;i++)
        //{
        //    float distance = Vector3.Distance(splineSamples[i].position, framePos);
        //    if(distance < minDistance)
        //    {
        //        minIndex = i;
        //        minDistance = distance;
        //    }
        //}

       return minIndex;
    }
}
