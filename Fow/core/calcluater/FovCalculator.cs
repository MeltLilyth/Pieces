using Assets.Scripts.Fow.core.calcluater.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fow.core.calcluater
{
	public abstract class FovCalculator : MaskCalcluatorBase
	{
		/// <summary>
		/// 需要遍历的点的数据
		/// </summary>
		protected Queue<FowMapPos> mapPosQueue = new Queue<FowMapPos>();
		
		/// <summary>
		/// 实时计算迷雾网格点数据
		/// </summary>
		protected sealed override void RealTimeCalclcator(FowFieldData fieldData)
		{
			Vector3 worldPosition = fieldData.position;
			float radiusSq = fieldData.radiusSquare;

			int x = Mathf.FloorToInt((worldPosition.x - base.beginPosition.x) * base.invDelta_x);
			int z = Mathf.FloorToInt((worldPosition.z - base.beginPosition.z) * base.invDelta_y);

			//当前传入的基点
			if (x < 0 || x >= base.tex_Width || z < 0 || z >= base.tex_Height) {
				return;
			}
			//当前传入的中心点为阻挡点
			if (base.mapData[x, z].isMapBlock) { return; }

			FowMapPos mapEntityPos = base.mapData[x, z];
			mapPosQueue.Enqueue(mapEntityPos);
			base.SetMapPosVisible(x, z);

			while (mapPosQueue.Count > 0) {
				var root = mapPosQueue.Dequeue();
				
				//当前点为阻挡点
				if (root.isMapBlock) {
					if (PreRayCast(root, mapEntityPos)) {
						if (root.IsMapPosShouldChcekAgain(base.checkInstanceId, base.checkTime, true)) {
							base.SetMapPosVisible(root.x, root.y);
						}
						else{
							this.RayCast(root, x, z, fieldData.radius);
						}
					}
					continue;
				}

				this.SetVisibleAtPosition(mapData[x - 1, z],  mapEntityPos, radiusSq);
				this.SetVisibleAtPosition(mapData[x , z - 1],  mapEntityPos, radiusSq);
				this.SetVisibleAtPosition(mapData[x + 1, z],  mapEntityPos, radiusSq);
				this.SetVisibleAtPosition(mapData[x , z + 1],  mapEntityPos, radiusSq);
			}
		}

		/// <summary>
		/// 判断并设定当前点的数据在地图上可见
		/// </summary>
		private void SetVisibleAtPosition(FowMapPos traverseMapPos, FowMapPos entityPos, float radius) {
			//不在渲染范围内
			if (traverseMapPos.x < 0 || traverseMapPos.x > base.tex_Width || traverseMapPos.y < 0 || traverseMapPos.y > base.tex_Height) {
				return;
			}
			//不在传入实体的视野范围内
			if (!base.IsMapPosInRange(traverseMapPos, entityPos, radius)) {
				return;
			}
			//是否需要再次检测
			if (!traverseMapPos.IsMapPosShouldChcekAgain(base.checkInstanceId, base.checkTime)) {
				return;
			}

			mapPosQueue.Enqueue(traverseMapPos);
			SetMapPosVisible(traverseMapPos.x, traverseMapPos.y);
		}

		/// <summary>
		/// 预检测边缘点 -- 碰撞边缘的模糊处理
		/// </summary>
		private bool PreRayCast(FowMapPos checkPos, FowMapPos centerPos) {
			float k = (float)((checkPos.x - centerPos.x) / (checkPos.y - centerPos.y));
			if (k < -0.414f && k >= -2.414f)
			{
				return !base.IsMapPosVisible(checkPos.x + 1, checkPos.y + 1) && !base.IsMapPosVisible(checkPos.x - 1, checkPos.y - 1);
			}
			else if (k < -2.414f || k >= 2.414f)
			{
				return !base.IsMapPosVisible(checkPos.x + 1, checkPos.y) && !base.IsMapPosVisible(checkPos.x - 1, checkPos.y);
			}
			else if (k < 2.414f && k >= 0.414f)
			{
				return !base.IsMapPosVisible(checkPos.x + 1, checkPos.y - 1) && !base.IsMapPosVisible(checkPos.x - 1, checkPos.y + 1);
			}
			else
			{
				return !base.IsMapPosVisible(checkPos.x, checkPos.y + 1) && !base.IsMapPosVisible(checkPos.x, checkPos.y - 1);
			}
		}

		/// <summary>
		/// 计算阻挡点和阻挡点后的某一片扇形区域的阴影
		/// </summary>
		protected abstract void RayCast(FowMapPos pos, int centX, int centZ, float radius);

		/// <summary>
		/// 计算阻挡点和阻挡点后的某一片扇形区域的阴影
		/// </summary>
		protected void SetInVisibleInline(int beginx, int beginy, int endx, int endy, FowMapPos entityPos, float radiusSq) {
			int dx = Mathf.Abs(endx - beginx);
			int dy = Mathf.Abs(endy - beginy);

			int forward = (endy < beginy && endx >= beginx) || (endy >= beginy && endx < beginx) ? -1 : 1;
			
			bool durTag = dy < dx;
			int twod = durTag ? 2 * dy : 2 * dx;
			int pass = durTag ? twod - dx : twod - dy;
			int twodm = durTag ? pass - dx : pass - dy;

			int pValue1, pValue2, to;
			if (durTag)
			{
				bool flag = beginx > endx;
				pValue1 = flag ? endx : beginx;
				pValue2 = flag ? endy : beginy;
				to = flag ? beginx : endx;
			}
			else {
				bool flag = beginy > endy;
				pValue1 = flag ? endy : beginy;
				pValue2 = flag ? endx : beginx;
				to = flag ? beginy : endy;
			}

			int x = durTag ? pValue1 : pValue2;
			int y = durTag ? pValue1 : pValue2;

			if (!SetMapPosInVisibleInline(mapData[x, y], entityPos, radiusSq)) { return; }

			while (pValue1 < to) {
				pValue1++;
				if (pass < 0) { 
					pass += twod; 
				}
				else {
					pValue2 += forward;
					pass += twodm;
				}

				x = durTag ? pValue1 : pValue2;
				y = durTag ? pValue1 : pValue2;

				if (!SetMapPosInVisibleInline(mapData[x, y], entityPos, radiusSq)) { return; }
			}
		}

		private bool SetMapPosInVisibleInline(FowMapPos mapPos, FowMapPos entityPos, float radiusSq) {
			//当前点不在检测范围内
			if (base.IsMapPosInRange(mapPos, entityPos, radiusSq))
			{
				return false;
			}
			//当前点已经判断为可见点
			if (!mapPos.IsMapPosShouldChcekAgain(base.checkInstanceId, base.checkTime, true))
			{
				return false;
			}
			mapPos.SetFowMapPosCheckTime(base.checkTime);

			return true;
		}

		public override void ReleaseCalclcator()
		{
			if (mapPosQueue != null) { mapPosQueue.Clear(); }
			mapPosQueue = null;
		}
	}
}
