using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeWall : MonoBehaviour
{
    [SerializeField]
    GameObject wall;

    [SerializeField]
    GameObject wallEditor;

    // Start is called before the first frame update
    void Start()
    {
        wall.SetActive(true);
        wallEditor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
