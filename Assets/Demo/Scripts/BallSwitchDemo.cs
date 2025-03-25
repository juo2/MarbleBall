using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSwitchDemo : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"collision:{collision.gameObject.name}");

        BallDemo ballDemo = collision.gameObject.GetComponent<BallDemo>();
        SwitchBall(ballDemo);
    }


    public void SwitchBall(BallDemo ballDemo)
    {
        Vector3 tempPos = ballDemo.transform.position;
        Quaternion tempQuaternion = ballDemo.transform.rotation;

        ballDemo.transform.position = transform.position;
        ballDemo.transform.rotation = transform.rotation;

        transform.position = tempPos;
        transform.rotation = tempQuaternion;

        ballDemo.velocity = ballDemo.velocity / 2f;
    }

}
