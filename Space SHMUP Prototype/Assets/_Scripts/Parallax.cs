using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject poi;
    public GameObject[] panels;
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f;

    private float panelHt;
    private float depth;

    void Start()
    {
        panelHt = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHt, depth);
    }

    void Update()
    {
        float ty, tx = 0;
        ty = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);

        if(poi != null)
        {
            tx = -poi.transform.position.x * motionMult;
        }

        panels[0].transform.position = new Vector3(tx, ty, depth);

        if(ty >= 0)
        {
            panels[1].transform.position = new Vector3(tx, ty - panelHt, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(tx, ty + panelHt, depth);
        }
    }
}
