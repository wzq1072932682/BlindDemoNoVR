using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostProcess_BlindEffect : MonoBehaviour
{
    public Material blindMat;
    public Material blurMat;
    public Material combineMat;
    public Vector3 spherePosition1;
    public float radius1 = 0.5f;
    public float softness = 0.5f;
    public Vector3 spherePosition2;
    public float radius2 = 0.5f;
    private Camera camera;

    public Texture noiseTexture;

    private RenderTexture buffer;
    private void OnEnable()
    {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.DepthNormals;
        
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (blindMat == null) return;
        var blindTexture = RenderTexture.GetTemporary(source.width, source.height);
        blindMat.SetVector("_Position1", spherePosition1);
        blindMat.SetFloat("_Radius1", radius1);
        blindMat.SetVector("_Position2", spherePosition2);
        blindMat.SetFloat("_Radius2", radius2);
        softness = Mathf.Clamp(softness,0.0f,50.0f);
        blindMat.SetFloat("_Softness", softness);
        //Camera.current  Camera.main camera

        
        Matrix4x4 vp = GL.GetGPUProjectionMatrix(Camera.current.projectionMatrix, false) * Camera.current.worldToCameraMatrix;
        vp=vp.inverse;
        Shader.SetGlobalMatrix("_InvVP", vp);
        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(Camera.current.projectionMatrix, true).inverse;
        Shader.SetGlobalMatrix("_ClipToView", vp);
        Graphics.Blit(source, blindTexture, blindMat,0);

        var maskTexture=RenderTexture.GetTemporary(source.width,source.height);
        blindMat.SetTexture("_NoiseTex", noiseTexture);
        Graphics.Blit(source, maskTexture, blindMat, 1);

        
        if (blurMat == null) return;
        var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(blindTexture, temporaryTexture, blurMat,0);
        var BlurOutlineTexture=RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(temporaryTexture, BlurOutlineTexture, blurMat,1);
        blindMat.SetTexture("_OutlineTex", blindTexture);
        blindMat.SetTexture("_OutlineBlurTex", BlurOutlineTexture);
        blindMat.SetTexture("_MaskTex",maskTexture);
        //blindMat.SetTexture("_StencilTex",buffer);
        //buffer = RenderTexture.GetTemporary(source.width, source.height, 24);
        //Graphics.SetRenderTarget(buffer.colorBuffer,source.depthBuffer);
        combineMat.SetTexture("_OutlineTex", blindTexture);
        combineMat.SetTexture("_OutlineBlurTex", BlurOutlineTexture);
        combineMat.SetTexture("_MaskTex", maskTexture);
       
        //Graphics.Blit(source, destination, blindMat, 2);
        Graphics.Blit(source, destination, combineMat);
        RenderTexture.ReleaseTemporary(temporaryTexture);
        RenderTexture.ReleaseTemporary(BlurOutlineTexture);
        RenderTexture.ReleaseTemporary(maskTexture);
        RenderTexture.ReleaseTemporary(blindTexture);
        //RenderTexture.ReleaseTemporary(buffer);
    }
    private void OnPostRender()
    {
        
    }

    public void SetMaterialParameter(Vector3 _position1, Vector3 _position2,float _radius1,float _radius2)
    {
        spherePosition1 = _position1;
        spherePosition2 = _position2;
        radius1 = _radius1;
        radius2 = _radius2;
    }
    

    
}
