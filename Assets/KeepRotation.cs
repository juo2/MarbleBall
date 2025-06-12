using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeepRotation : MonoBehaviour
{
    [SerializeField]
    Transform traceing;

    public Vector3 pos;

    public Text label;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = traceing.transform.position + pos;
    }

    public void SetLabel(int num)
    {
        label.text = num.ToString();
    }
}
