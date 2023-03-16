using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.scripts.pipeline
{
    public class GPUDrivenFeature : ScriptableRendererFeature
    {
        class GPUDrivenRenderPipeline : ScriptableRenderPass
        {
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get("GPU Driven Scene Pipeline");
                List<Mesh> meshs = SceneManager.Instance.F_GetMeshesInCameraProjection();
                Mesh mesh = new Mesh();
                CombineInstance[] combineInstances = new CombineInstance[meshs.Count];
                for (int i = 0; i < combineInstances.Length; i++) {
                    combineInstances[i].mesh = meshs[i];
                    combineInstances[i].transform = Matrix4x4.identity;
                }

                mesh.CombineMeshes(combineInstances);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                commandBuffer.DrawMeshInstanced(mesh, 0, SceneManager.Instance.Material, 0, new Matrix4x4[1] { new GameObject().transform.localToWorldMatrix }, 1);
                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            throw new NotImplementedException();
        }

        public override void Create()
        {
            throw new NotImplementedException();
        }
    }
}
