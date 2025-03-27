using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDemo : MonoBehaviour
{
    [SerializeField]
    Rigidbody m_rigidbody;

    [SerializeField]
    Light pointLight;

    [SerializeField]
    Transform effectTran;

    [SerializeField]
    Transform effectTran2;

    List<Color> lightColorList = new List<Color>()
    {
        new Color(83f/255f,242f/255f,255f/255f), //蓝
        new Color(215f/255f,255f/255f,0f),       //黄
        new Color(255f/255f,81f/255f,0f),        //红
    };

    public Vector3 velocity
    {
        get
        {
            return m_rigidbody.velocity;
        }
        set
        {
            m_rigidbody.velocity = value;
        }
    }

    public float speed
    {
        get
        {
            return m_rigidbody.velocity.magnitude;
        }
    }

    public Vector3 angularVelocity
    {
        get
        {
            return m_rigidbody.angularVelocity;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        m_rigidbody.velocity = velocity;
    }

    public void SetAngularVelocity(Vector3 angularVelocity)
    {
        m_rigidbody.angularVelocity = angularVelocity;
    }
    
    public void AddForce(Vector3 force)
    {
        m_rigidbody.AddForce(force);
    }

    public void ResetState(PhysicsState ps = null)
    {
        transform.position = ps != null ? ps.position : Vector3.zero;
        transform.rotation = ps != null ? ps.rotation : Quaternion.identity;
        m_rigidbody.velocity = ps != null ? ps.velocity : Vector3.zero;
        m_rigidbody.angularVelocity = ps != null ? ps.angularVelocity : Vector3.zero;
    }

    public void SetPointLight(int _index)
    {
        //pointLight.color = lightColorList[_index];
    }

    void OnDrawGizmos()
    {
        // 获取刚体组件
        if (m_rigidbody != null)
        {
            // 设置 Gizmos 颜色（可以更改颜色）
            Gizmos.color = Color.red;

            // 绘制从物体位置到速度方向的线段
            Gizmos.DrawLine(transform.position, transform.position + m_rigidbody.velocity);
        }
    }

    public void PlayEffet()
    {
        if(SkinManager.Instance.isDay())
        {
            ParticleSystem[] particleSystems = effectTran2.GetComponentsInChildren<ParticleSystem>();

            foreach (var p in particleSystems)
            {
                ParticleSystem.EmissionModule emissionModule = p.emission;
                emissionModule.rateOverDistance = 20.0f;
            }
        }
        else
        {
            ParticleSystem[] particleSystems = effectTran.GetComponentsInChildren<ParticleSystem>();

            foreach (var p in particleSystems)
            {
                ParticleSystem.EmissionModule emissionModule = p.emission;
                emissionModule.rateOverDistance = 20.0f;
            }
        }
        
    }

    public void StopEffect()
    {
        ParticleSystem[] particleSystems = effectTran.GetComponentsInChildren<ParticleSystem>();

        foreach (var p in particleSystems)
        {
            ParticleSystem.EmissionModule emissionModule = p.emission;
            emissionModule.rateOverDistance = 0.0f;
        }

        particleSystems = effectTran2.GetComponentsInChildren<ParticleSystem>();

        foreach (var p in particleSystems)
        {
            ParticleSystem.EmissionModule emissionModule = p.emission;
            emissionModule.rateOverDistance = 0.0f;
        }
        
    }

}
