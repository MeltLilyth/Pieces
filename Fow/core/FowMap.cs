using Assets.Scripts.Fow.core.calcluater;
using Assets.Scripts.Fow.core.calcluater.entity;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Fow.core
{
	public class FowMap
	{
		/// <summary>
		/// Fov计算器对象处理
		/// </summary>
		private MaskCalcluatorBase calcluatorBase = null;

		/// <summary>
		/// 唯一锁相关数据计算
		/// </summary>
		private Mutex mutex_calcluator = new Mutex();

		public FowMap(Vector3 beginPosition, float xsize, float zsize, int texWidth, int texHeight, float heightRange) {
			if (calcluatorBase == null) {
				calcluatorBase = new FovSimple();
			}

			float deltax = xsize / texWidth;
			float deltay = zsize / texHeight;
			float invDeltax = 1.0f / deltax;
			float invDeltay = 1.0f / deltay;

			if (calcluatorBase != null) { 
				calcluatorBase.init_MaskCalcluator(deltax, deltay, invDeltax, invDeltay, texWidth, texHeight, heightRange, beginPosition); 
			}
		}

		/// <summary>
		/// 外部接口调用
		/// </summary>
		/// <param name="fieldDatas"></param>
		public void SetVisibleInMap(List<FowFieldData> fieldDatas) { this.CalculateFov(fieldDatas); }

		/// <summary>
		/// 获得战争迷雾纹理
		/// </summary>
		public bool RefreshFOWTexture(){ return calcluatorBase != null && calcluatorBase.RefreshTexture(); }

		/// <summary>
		/// 计算迷雾Fow相关信息
		/// </summary>
		private void CalculateFov(Dictionary<long, FowFieldData> fowInfos) {
			if (fowInfos == null || fowInfos.Count == 0 || calcluatorBase == null) {
				return;
			}

			//遍历并计算相关迷雾数据
			short checkInstanceId = 0;
			foreach (var info in fowInfos) {
				if (info.Value == null) { continue; }

				checkInstanceId += 1;
				calcluatorBase.Calclcator(info.Value, checkInstanceId);
			}
			//数据标记为已更新
			calcluatorBase.MarkAsUpdate();
		}

		private void CalculateFov(List<FowFieldData> fowInfos) {
			if (fowInfos == null || fowInfos.Count == 0 || calcluatorBase == null)
			{
				return;
			}

			//遍历并计算相关迷雾数据
			short checkInstanceId = 0;
			foreach (var info in fowInfos)
			{
				if (info == null) { continue; }

				checkInstanceId += 1;
				calcluatorBase.Calclcator(info, checkInstanceId);
			}
			//数据标记为已更新
			calcluatorBase.MarkAsUpdate();
		}

		/// <summary>
		/// 释放对象数据引用
		/// </summary>
		public void ReleaseFowMap() {
			if (calcluatorBase != null) { 
				calcluatorBase.ReleaseCalclcator();
				calcluatorBase = null;
			}
		}

		// <summary>
		/// 获得战争迷雾纹理
		/// </summary>
		/// <returns></returns>
		public Texture2D GetFOWTexture(){ return calcluatorBase.maskTexture.maskTexture; }

		public bool IsVisibleInMap(int x, int z) { return calcluatorBase.maskTexture.isVisiable(x, z); }

	}
}
