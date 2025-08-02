
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PhotoInteractable : UdonSharpBehaviour
{
    [SerializeField] private RenderTexture cameraRenderTexture;
    [SerializeField] private Application application;

    public override void Interact()
    {
        RenderTexture capturedTexture = new RenderTexture(cameraRenderTexture.width, cameraRenderTexture.height, cameraRenderTexture.depth);
        capturedTexture.format = cameraRenderTexture.format;
        VRCGraphics.Blit(cameraRenderTexture, capturedTexture);
    }
}
