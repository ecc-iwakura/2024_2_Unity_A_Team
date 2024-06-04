using UnityEngine;

public class WaveEffect : MonoBehaviour
{
    public Material transitionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (transitionMaterial != null)
        {
            Graphics.Blit(src, dest, transitionMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
