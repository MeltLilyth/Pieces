using Assets.scripts.core;
using Assets.scripts.terrain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts
{
    public class SceneManager : MonoBehaviour
    {
        private static SceneManager _instance;
        public static SceneManager Instance {
            get {
                if (_instance == null) {
                    _instance = new GameObject("SceneManager").AddComponent<SceneManager>();
                    GameObject.DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

        //当前地图地表mesh信息
        private List<Mesh> currentMapMeshInfos = new List<Mesh>();
        //当前场景使用相机预设
        private Camera _MainCamera = null;
        //当前地图Id信息
        private long currentMapId = -1;
        //加载标识
        private bool isLoadMeshInfos = false;
        
        public Material Material { get => new Material(Shader.Find("Universal Render Pipeline/Lit")); }

        public bool IsLoadMeshInfos { get => isLoadMeshInfos; }

        public void F_LoadNewMap(int mapId){
            if (mapId == currentMapId) {
                UnityEngine.Debug.LogErrorFormat("请求加载的是同一张地图, 不会进行处理. MapId - {0}, currentMapId - {1}", mapId, currentMapId);
                return; 
            } 
            //测试案例 - 这边就用测试代码了,正式项目的话可以单独提出来写一个方法
            this._MainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
            if (this._MainCamera != null) {
                this.currentMapMeshInfos.Clear();

                this.isLoadMeshInfos = true;
                for (int i = 0; i < 64; i++) {
                    using (FileStream fileStream = new FileStream(Application.dataPath + "/resources/mapTerrain_" + (i + 1).ToString() + ".x", FileMode.Open, FileAccess.Read)) {
                        try
                        {
                            Vector3[] vertices = TextEncoding.DecodingArray<Vector3>(fileStream);
                            Vector3[] normals = TextEncoding.DecodingArray<Vector3>(fileStream);
                            Vector2[] uvs = TextEncoding.DecodingArray<Vector2>(fileStream);
                            Vector4[] tangents = TextEncoding.DecodingArray<Vector4>(fileStream);
                            int[] triangles = TextEncoding.DecodingArray<int>(fileStream);

                            float[] val = TextEncoding.DecodingVector(fileStream, typeof(Vector3));
                            Vector3 center = val.Length == 3 ? new Vector3(val[0], val[1], val[2]) : Vector3.zero;

                            MeshInfo meshInfo = new MeshInfo(vertices, normals, uvs, tangents, triangles, center);
                            this.currentMapMeshInfos.Add(meshInfo.Mesh);
                        }
                        catch (Exception ex) { UnityEngine.Debug.LogException(ex); }
                        finally { fileStream.Close(); }
                    }
                }
                this.isLoadMeshInfos = false;
            }
        }

        public List<Mesh> F_GetMeshesInCameraProjection() {
            List<Mesh> infos = new List<Mesh>();
            
            Matrix4x4 matrixWToCam = this._MainCamera.worldToCameraMatrix;
            Matrix4x4 matrixCamToProjection = Matrix4x4.Perspective(_MainCamera.fieldOfView, _MainCamera.aspect, _MainCamera.nearClipPlane, _MainCamera.farClipPlane);
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(matrixCamToProjection * matrixWToCam);
            
            for (int i = 0; i < currentMapMeshInfos.Count; i++) {
                if (GeometryUtility.TestPlanesAABB(planes, currentMapMeshInfos[i].bounds)) {
                    infos.Add(currentMapMeshInfos[i]);
                }    
            }
            return infos;
        }        
    }
}
