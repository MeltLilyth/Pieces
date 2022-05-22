using Assets.Scripts.Fow.core.calcluater.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fow.core.calcluater
{
	public abstract class MaskCalcluatorBase
	{
		protected float delta_x;
		protected float delta_y;
		protected float invDelta_x;
		protected float invDelta_y;
		protected int tex_Width;
		protected int tex_Height;
		protected Vector3 beginPosition;

		/// <summary>
		/// 地图信息数据
		/// </summary>
		protected IFowMapData mapData;

		/// <summary>
		/// 迷雾贴图渲染对象
		/// </summary>
		public FowMaskTexture maskTexture;

		/// <summary>
		/// 是否已经初始化完成
		/// </summary>
		private bool isInit = false;

		/// <summary>
		/// 当前检测数据次数
		/// </summary>
		protected short checkTime = 0;

		/// <summary>
		/// 当前检测对象的唯一标识
		/// </summary>
		protected short checkInstanceId = 0;

		/// <summary>
		/// 初始化迷雾组件计算器
		/// </summary>
		public virtual void init_MaskCalcluator(float deltax, float deltay, float invDeltax, float invDeltay, int texWidth, int texHeight, float heightRange, Vector3 beginPosition)
		{
			this.delta_x = deltax;
			this.delta_y = deltay;
			this.invDelta_x = invDeltax;
			this.invDelta_y = invDeltay;

			this.tex_Height = texHeight;
			this.tex_Width = texWidth;
			this.beginPosition = beginPosition;

			//初始化地图信息数据
			mapData = new FowMapData(texWidth, texHeight);
			maskTexture = new FowMaskTexture(texWidth, texHeight);

			if (!isInit) { this.isInit = true; }
		}

		/// <summary>
		/// 外部调用计算迷雾数据
		/// </summary>
		public void Calclcator(FowFieldData fieldData, short instanceId) {
			if (!isInit) { return; }

			this.checkInstanceId = instanceId;
			checkTime = checkTime == short.MaxValue? (short)0 : (short)(checkTime + 1);			
			
			this.RealTimeCalclcator(fieldData);
		}

		/// <summary>
		/// 标记为需要更改
		/// </summary>
		public void MarkAsUpdate() { if (maskTexture != null) { maskTexture.MarkAsUpdate(); } }

		/// <summary>
		/// 
		/// </summary>
		public bool RefreshTexture() { return maskTexture != null && maskTexture.RefreshMaskTexture(); }

		/// <summary>
		/// 设定当前点在迷雾区域内可见
		/// </summary>
		protected void SetMapPosVisible(int x, int z) {
			mapData[x, z].SetFowMapPosVisiable(checkTime, checkInstanceId);
			if (maskTexture != null) { 
				maskTexture.SetAsVisiable(x, z); 
			}
		}

		/// <summary>
		/// 判断某一点mapPos是否在实体对象entityPos的范围内
		/// </summary>
		protected bool IsMapPosInRange(FowMapPos mapPos, FowMapPos entityPos, float radiusSq) {
			float mapX = (mapPos.x - entityPos.x) * delta_x;
			float mapZ = (mapPos.y - entityPos.y) * delta_y;
			//二维向量模长的平方
			float magnitude = Mathf.Pow(mapX, 2) + Mathf.Pow(mapZ, 2);
			
			return magnitude < radiusSq;
		}

		protected bool IsMapPosVisible(int x, int y) {
			if (x < 0 || x > tex_Width || y < 0 || y > tex_Height) {
				return false;
			}
			return mapData[x, y].isMapBlock;
		}

		/// <summary>
		/// 实时计算迷雾数据逻辑
		/// </summary>
		protected abstract void RealTimeCalclcator(FowFieldData fieldData);

		/// <summary>
		/// 清除缓存数据
		/// </summary>
		public abstract void ReleaseCalclcator();
	}
}
