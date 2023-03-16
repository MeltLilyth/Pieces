using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private int instanceCount = 100000;
    private Mesh instanceMesh = null;
    private Material instanceMaterial = null;
    private int subMeshIndex = 0;

    private int cachedInstanceCount = -1;
    private int cachedSubMeshIndex = -1;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;

    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private void TestImplement() {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
    }

    void UpdateBuffer() { 
        if(instanceMesh != null) { 
            subMeshIndex = Mathf.Clamp(subMeshIndex, 0, instanceMesh.subMeshCount - 1); 
        }
        //释放已占有的数据
        if (positionBuffer != null) { positionBuffer.Release(); }
        positionBuffer = new ComputeBuffer(instanceCount, 16);
        Vector4[] positions = new Vector4[instanceCount];
        float distance = 0, angle = 0;
        for (int i = 0; i < instanceCount; i++) {
            distance = Random.Range(20.0f, 100.0f);
            angle = Random.Range(0.0f, Mathf.PI * 2.0f);
            positions[i] = new Vector4(
                                Mathf.Sin(angle) * distance,
                                Random.Range(-2.0f, 2.0f),
                                Mathf.Cos(angle) * distance,
                                Random.Range(0.05f, 0.25f)
                               );
        }
        positionBuffer.SetData(positions);
        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);
        
    }
}
