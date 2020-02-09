using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracer : MonoBehaviour
{
    [Header("Settings")]
    [Range(1,40)]
    public int BOUNCES;
    public int res = 1;

    public double multiplier;
    [Header("Other Referencs")]
    public ComputeShader raytracer;
    public bool compute;
    public Texture SkyboxTexture;
    private RenderTexture result;

    List<SphereData> spheres = new List<SphereData>();



    public void RegisterSphere (RaySphere s)
    {
        SphereData d = new SphereData();
        d.col = new Vector3(s.renderColor.r, s.renderColor.g, s.renderColor.b);
        d.pos = (s.transform.position);

        //s.GetComponent<SphereCollider>().radius * (float)multiplier *
        d.details = new Vector3( s.transform.localScale.x / 2, s.shininess, s.specIntensity);

        spheres.Add(d);
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (compute)
        {
            try
            {
                //source is the cam tex
                if (result == null || result.width != Screen.width / res || result.height != Screen.height / res)
                {
                    if (result != null) result.Release();

                    result = new RenderTexture(Screen.width/res, Screen.height / res, 8, RenderTextureFormat.ARGBFloat, 0);
                    result.enableRandomWrite = true;
                    result.Create();

                }

           
                //12 bytes for 1 vector  * 3 elements
                ComputeBuffer buffer = new ComputeBuffer(spheres.Count, 12 * 3);
                buffer.SetData(spheres);

                raytracer.SetTexture(0, "Result", result);
                raytracer.SetBuffer(0, "SphereData", buffer);
                raytracer.SetTexture(0, "_SkyboxTexture", SkyboxTexture);

                raytracer.SetInt("_bounces", BOUNCES);

                raytracer.SetVector("CameraPos", Camera.main.transform.position);
                raytracer.SetMatrix("_CameraToWorld", Camera.main.cameraToWorldMatrix);
                raytracer.SetMatrix("_ScreenToCamera", Camera.main.projectionMatrix.inverse);

                raytracer.Dispatch(0, (Screen.width / res + 7) / 8, (Screen.height / res + 7) / 8, 1);

            }
            catch
            {

            }
            //destination is what's rendered
            Graphics.Blit(result, destination);
            
            spheres.Clear();

            
            
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    [Header("Debug")]
    int lastBounce = 0;

    float avgFps;
    float fps = 0;
    int totalFramesCaptured = 0;
    private void Update()
    {
        
        

        if (BOUNCES != lastBounce)
        {
            //Print
            
            Debug.Log("Average FPS For " + lastBounce + " bounces is : " + avgFps);
            
            fps = 0;
            avgFps = 0;
            totalFramesCaptured = 0;

            lastBounce = BOUNCES;
        }

        else
        {
            fps += (1f / Time.unscaledDeltaTime);
            
            avgFps = fps/totalFramesCaptured;
            totalFramesCaptured += 1;
        }


    }

}
