using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts.terrain
{
    public class TerrainUtils
    {
        /**关于mesh分割的相关算法 --
          *   
          *  遍历mesh中所有的三角形,会出现下面几种情况:
          * 1.三角形的三个顶点都在数据集合1中, 将三角形的三个顶点引用的数据记录到数据集合1
          * 2.三角形的三个顶点都在数据集合2中, 将三角形的三个顶点引用的数据记录到数据集合2
          * 3.当前遍历的三角形与切割面相交 -- 涉及到创建新的三角形
          * 
          *     unity 中的三角形的顶点顺序一般为顺时针, 顺时针代表正面, 逆时针代表背面(unity 默认只渲染正面不渲染背面)
          *       比如长方形ABCD --> mesh中的triangles的数据为 
          *         mesh.vertices = new Vector3{ posA, posB, posC, posD };
          *         mesh.triangles = new int[]{0, 2, 3, 0, 1, 2 };
          *         D --- A
          *         |     |     绘制出来的长方形顶点信息大概是这样
          *         C --- B
          *     
          *     可以根据mesh中三角形的顶点绘制顺序去判断三角形的相关分割
          */

        public static List<GameObject> TerrainMeshClipping(GameObject inst, int trav, Material material = null) {
            MeshFilter meshFilter;
            if (inst == null || (meshFilter = inst.gameObject.GetComponent<MeshFilter>()) == null || (meshFilter.mesh == null && meshFilter.sharedMesh == null)) {
                return null;
            }
            Mesh mesh = meshFilter.mesh == null ? meshFilter.sharedMesh : meshFilter.mesh;
            bool flag = trav == 0 || trav % 2 == 0;

            bool[] aboves = new bool[mesh.vertices.Length];
            int[] newTriangles = new int[mesh.vertices.Length];

            MeshInfo info1 = new MeshInfo();
            MeshInfo info2 = new MeshInfo();

            //顶点数据
            for (int i = 0; i < mesh.vertices.Length; i++) {
                aboves[i] = flag ? mesh.vertices[i].x > mesh.bounds.center.x : mesh.vertices[i].z > mesh.bounds.center.z;
                newTriangles[i] = (aboves[i] ? info1 : info2).vertexcount;
                (aboves[i] ? info1 : info2).AddVertex(mesh.vertices[i], mesh.normals[i], mesh.uv[i], mesh.tangents[i]);
            }

            TerrainMeshTriangleCipping(mesh, flag, aboves, newTriangles, ref info1, ref info2);
            return new List<GameObject>() {
                GernerateMeshClipInstance(info1, material),
                GernerateMeshClipInstance(info2, material),
            };
        }

        private static void TerrainMeshTriangleCipping(Mesh mesh, bool flag, bool[] aboves, int[] newTriangles, ref MeshInfo info1, ref MeshInfo info2) {
            int i0, i1, i2, up, down0, down1;
            for (int i = 0; i < mesh.triangles.Length / 3; i++) {
                i0 = mesh.triangles[i * 3];
                i1 = mesh.triangles[i * 3 + 1];
                i2 = mesh.triangles[i * 3 + 2];

                if (aboves[i0] && aboves[i1] && aboves[i2])
                {
                    info1.AddTriangle(new List<int>() { newTriangles[i0], newTriangles[i1], newTriangles[i2] });
                }
                else if (!aboves[i0] && !aboves[i1] && !aboves[i2])
                {
                    info2.AddTriangle(new List<int>() { newTriangles[i0], newTriangles[i1], newTriangles[i2] });
                }
                else {
                    //当前三角形被中线分割，需要重新生成三角形 -- 一个三角形被线段划分会生成3个三角形数据
                    bool flag1 = aboves[i2] == aboves[i1] && aboves[i1] != aboves[i0];
                    bool flag2 = aboves[i0] == aboves[i2] && aboves[i1] != aboves[i2];

                    up = flag1 ? i0 : flag2 ? i1 : i2;
                    down0 = flag1 ? i1 : flag2 ? i2 : i0;
                    down1 = flag1 ? i2 : flag2 ? i0 : i1;

                    if (aboves[up])
                    {
                        TerrainMeshTriangleSplit(mesh, flag, newTriangles, up, down0, down1, ref info1, ref info2);
                    }
                    else {
                        TerrainMeshTriangleSplit(mesh, flag, newTriangles, up, down0, down1, ref info2, ref info1);
                    }
                }
            }
        }

        private static void TerrainMeshTriangleSplit(Mesh mesh, bool flag, int[] newTriangles ,int up, int down0, int down1, ref MeshInfo info1, ref MeshInfo info2) {
            Vector3 normal = flag ? Vector3.right : Vector3.forward;
            float updot = Mathf.Abs(Vector3.Dot((mesh.bounds.center - mesh.vertices[up]), normal));
            float downdot0 = Mathf.Abs(Vector3.Dot((mesh.vertices[down0] - mesh.vertices[up]), normal));
            float downdot1 = Mathf.Abs(Vector3.Dot((mesh.vertices[down1] - mesh.vertices[up]), normal));

            float ascale = updot / downdot0;
            float bscale = updot / downdot1;

            //vertex
            Vector3 pos_a = mesh.vertices[up] + (mesh.vertices[down0] - mesh.vertices[up]) * ascale;
            Vector3 pos_b = mesh.vertices[up] + (mesh.vertices[down1] - mesh.vertices[up]) * bscale;

            //normal
            Vector3 normal_a = mesh.normals[up] + (mesh.normals[down0] - mesh.normals[up]) * ascale;
            Vector3 normal_b = mesh.normals[up] + (mesh.normals[down1] - mesh.normals[up]) * bscale;
            
            //uv
            Vector2 uv_a = mesh.uv[up] + (mesh.uv[down0] - mesh.uv[up]) * ascale;
            Vector2 uv_b = mesh.uv[up] + (mesh.uv[down1] - mesh.uv[up]) * bscale;

            //tanget
            Vector4 tangent_a = mesh.tangents[up] + (mesh.tangents[down0] - mesh.tangents[up]) * ascale;
            tangent_a.w = mesh.tangents[down0].w;
            Vector4 tangent_b = mesh.tangents[up] + (mesh.tangents[down1] - mesh.tangents[up]) * bscale;
            tangent_b.w = mesh.tangents[down1].w;

            int idx_a = info1.vertexcount;
            info1.AddVertex(pos_a, normal_a, uv_a, tangent_a);
            int idx_b = info1.vertexcount;
            info1.AddVertex(pos_b, normal_b, uv_b, tangent_b);
            info1.AddTriangle(new int[3] {newTriangles[up], idx_a, idx_b });

            idx_a = info2.vertexcount;
            info2.AddVertex(pos_a, normal_a, uv_a, tangent_a);
            idx_b = info2.vertexcount;
            info2.AddVertex(pos_b, normal_b, uv_b, tangent_b);
            info2.AddTriangle(new int[6] { idx_b, idx_a, newTriangles[down0], idx_b, newTriangles[down0], newTriangles[down1] });
        }

        private static GameObject GernerateMeshClipInstance(MeshInfo meshInfo, Material material = null) {
            GameObject meshInstance = new GameObject("meshInstance-clone");
            if (meshInfo != null) {
                meshInstance.AddComponent<MeshFilter>().mesh = meshInfo.Mesh;
                meshInstance.AddComponent<MeshRenderer>().material = material == null ? new Material(Shader.Find("Universal Render Pipeline/Lit")) : material; 
            }
            return meshInstance;
        }

    }
}
