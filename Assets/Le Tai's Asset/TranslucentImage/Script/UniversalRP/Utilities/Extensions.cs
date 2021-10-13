using UnityEngine;
using UnityEngine.Rendering;

namespace LeTai.Asset.TranslucentImage.UniversalRP
{
public static class Extensions
{
    public static void BlitProcedural(this CommandBuffer     cmd,
                                      RenderTargetIdentifier source,
                                      RenderTargetIdentifier destination,
                                      Material               material,
                                      int                    passIndex)
    {
        cmd.SetGlobalTexture(ShaderId.MAIN_TEX, source);
        cmd.SetRenderTarget(new RenderTargetIdentifier(destination, 0, CubemapFace.Unknown, -1),
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.Store,
                            RenderBufferLoadAction.DontCare,
                            RenderBufferStoreAction.DontCare);
        cmd.DrawProcedural(Matrix4x4.identity, material, passIndex, MeshTopology.Quads, 4, 1, null);
    }
}
}
