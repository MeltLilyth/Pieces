using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.terrain
{
    public class MeshInfo
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector2> uvs = new List<Vector2>();
        private List<Vector4> tangents = new List<Vector4>();
        private List<int> triangles = new List<int>();
        private Vector3 center = default(Vector3);


        public int vertexcount { get => this.vertices.Count; }

        public MeshInfo() { }

        public MeshInfo(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, Vector4[] tangents, int[] triangles, Vector3 center) {
            this.vertices.AddRange(vertices);
            this.normals.AddRange(normals);
            this.uvs.AddRange(uvs);
            this.tangents.AddRange(tangents);
            this.triangles.AddRange(triangles);
            this.center = center;
        }

        public Mesh Mesh {
            get{
                Mesh mesh = new Mesh();
                mesh.vertices = this.vertices.ToArray();
                mesh.normals = this.normals.ToArray();
                mesh.uv = this.uvs.ToArray();
                mesh.tangents = this.tangents.ToArray();
                mesh.triangles = this.triangles.ToArray();

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();  //这个是建议调用一下，可能会出现Mesh裁切结束之后边缘黑边的情况 -- 一般来说黑边的情况就是法线数据不对

                return mesh;
            }
        }

        public void AddVertex(Vector3 vertex, Vector3 normal, Vector2 uv, Vector4 tangent) {
            this.vertices.Add(vertex);
            this.normals.Add(normal);
            this.uvs.Add(uv);
            this.tangents.Add(tangent);
        }

        public void AddTriangle(List<int> indices) { this.triangles.AddRange(indices); }

        public void AddTriangle(int[] indices) { this.triangles.AddRange(indices); }
    }
}
