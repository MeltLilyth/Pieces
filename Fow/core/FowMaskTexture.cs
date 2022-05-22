using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fow.core
{
	public class FowMaskTexture
	{
		/// <summary>
		/// 当前蒙版贴图的渲染状态
		/// </summary>
		private enum UpdateMark {
			None,
			Changed,
			EndUpdate,
		}
		private UpdateMark updateMark = UpdateMark.None;

		/// <summary>
		/// 蒙版贴图数据
		/// </summary>
		public Texture2D maskTexture { private set; get; }

		/// <summary>
		/// 地图长度数据
		/// </summary>
		private int texWidth;

		/// <summary>
		/// 地图宽度数据
		/// </summary>
		private int texHeight;

		/// <summary>
		/// 地图缓存数据
		/// </summary>
		private byte[] maskCache;

		/// <summary>
		/// 地图数据
		/// </summary>
		private Color[] colorBuffer;

		/// <summary>
		/// 唯一锁对象
		/// </summary>
		private Mutex mutex_ColorUpdate = new Mutex();

		public FowMaskTexture(int texWidth, int texHeight)
		{
			this.texWidth = texWidth;
			this.texHeight = texHeight;

			maskCache = new byte[texWidth * texHeight];
			colorBuffer = new Color[texWidth * texHeight];
		}

		/// <summary>
		/// 将地图的某一个点的数据设置为阻挡点
		/// </summary>
		public void SetAsVisiable(int x, int z) {
			maskCache[z * texWidth + x] = 1;
			updateMark = UpdateMark.Changed;
		}

		/// <summary>
		/// 将检测结果处理到对应的数据
		/// </summary>
		public void MarkAsUpdate() {
			if (updateMark != UpdateMark.Changed) { updateMark = UpdateMark.EndUpdate; return; }
			//获取唯一锁对象
			mutex_ColorUpdate.WaitOne();
			for (int i = 0; i < texWidth; i++) {
				for (int j = 0; j < texHeight; j++) {
					int index = j * texWidth + i;
					if (index < 0 || index > colorBuffer.Length || index > maskCache.Length) {
						continue;						
					}
					bool isVisible = maskCache[index] == 1;
					
					Color origin = colorBuffer[index];
					origin.r = Mathf.Clamp01(origin.r + origin.g);
					origin.b = origin.g;
					origin.g = isVisible ? 1 : 0;

					colorBuffer[index] = origin;
					maskCache[index] = 0;
				}
			}
			mutex_ColorUpdate.ReleaseMutex();
		}

		/// <summary>
		/// 更新渲染迷雾贴图
		/// </summary>
		public bool RefreshMaskTexture() {
			//如果状态没有改变，就代表没有数据需要更新
			if (updateMark == UpdateMark.None) { return false; }

			//当前的数据已经更新完成
			if (updateMark == UpdateMark.EndUpdate) { return true; }

			//如果当前的MaskTexture对象为空，则直接生成一张新的
			if(maskTexture == null) {
				maskTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGB24, false);
				maskTexture.wrapMode = TextureWrapMode.Clamp;
			}

			//将计算的渲染数据填充进入maskTexture对象 -- 数据获取因数据同步需要进行锁管理
			mutex_ColorUpdate.WaitOne();

			maskTexture.SetPixels(colorBuffer);
			maskTexture.Apply();
			updateMark = UpdateMark.None;

			mutex_ColorUpdate.ReleaseMutex();

			return true;
		}

		/// <summary>
		/// 判断当前点坐标是否可见
		/// </summary>
		public bool isVisiable(int x, int z) {
			if (x < 0 || x > texWidth || z < 0 || z > texHeight) {
				return false;
			}
			return colorBuffer[z * texWidth + z].g > 0.5f;
		}

		/// <summary>
		/// 释放对象引用资源
		/// </summary>
		public void ReleaseMaskTexture() {
			if (maskTexture != null) { GameObject.Destroy(maskTexture); }

			maskTexture = null;
			maskCache = null;
			colorBuffer = null;
		}

	}
}
