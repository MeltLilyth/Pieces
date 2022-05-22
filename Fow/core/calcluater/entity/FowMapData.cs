using ASL.FogOfWar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Fow.core.calcluater.entity
{
	public class FowMapData : IFowMapData
	{
		/// <summary>
		/// 地图长度
		/// </summary>
		protected int texWidth;
		/// <summary>
		/// 地图宽度
		/// </summary>
		protected int texHeight;

		/// <summary>
		/// 地图数据信息
		/// </summary>
		protected FowMapPos[,] mapData;

		public FowMapPos this[int x, int z] {
			get {
				if (x < 0 || x > texWidth || z < 0 || z > texHeight) {
					return null;
				}
				return mapData[x, z];
			}
		}

		public FowMapData(int tex_Width, int tex_Height) {
			this.texWidth = tex_Width;
			this.texHeight = tex_Height;

			mapData = new FowMapPos[texWidth, texHeight];
		}

		/// <summary>
		/// 生成地图数据
		/// </summary>
		public void GenerateMapData(float beginx, float beginy, float deltax, float deltay, float heightRange){
			for (int i = 0; i < texWidth; i++) {
				for (int j = 0; j < texHeight; j++) {
					mapData[i, j] = new FowMapPos(i , j, FOWUtils.IsObstacle(beginx, beginy, deltax, deltay, heightRange, i, j));
				}
			}
		}


	}
}
