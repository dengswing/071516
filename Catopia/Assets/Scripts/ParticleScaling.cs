using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ParticleScaling : MonoBehaviour
{
    public void OnWillRenderObject()
    {
        //Vector3 center =  Camera.current.worldToCameraMatrix.MultiplyPoint3x4(transform.position);

        //GetComponent<ParticleRenderer>().material.SetVector("_Center", center);

        GetComponent<ParticleRenderer>().material.SetVector("_Center", transform.position);
        GetComponent<ParticleRenderer>().material.SetVector("_Scaling", transform.lossyScale);
        GetComponent<ParticleRenderer>().material.SetMatrix("_Camera", Camera.current.worldToCameraMatrix);
        GetComponent<ParticleRenderer>().material.SetMatrix("_CameraInv", Camera.current.worldToCameraMatrix.inverse);
    }
}
