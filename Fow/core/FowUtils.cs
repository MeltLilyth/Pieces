﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fow.core
{
	public class FowUtils
	{
		/// <summary>
		/// Editor使用 -- 场景中绘制迷雾区域网格
		/// </summary>
		public static void DrawFogOfWarGizmos(Vector3 position, float xSize, float zSize, int texWidth, int texHeight,
			float heightRange)
		{
			if (heightRange <= 0)
				return;
			if (xSize <= 0 || zSize <= 0 || texWidth <= 0 || texHeight <= 0)
				return;
			Gizmos.color = Color.green;

			float deltax = xSize / texWidth;
			float deltay = zSize / texHeight;

			Vector3 origin = position - new Vector3(xSize / 2, 0, zSize / 2);

			for (int i = 0; i <= texWidth; i++)
			{
				Vector3 b = origin + new Vector3(i * deltax, 0, 0);
				Vector3 t = origin + new Vector3(i * deltax, 0, zSize);
				Gizmos.DrawLine(b, t);
			}
			for (int j = 0; j <= texHeight; j++)
			{
				Vector3 b = origin + new Vector3(0, 0, j * deltay);
				Vector3 t = origin + new Vector3(xSize, 0, j * deltay);
				Gizmos.DrawLine(b, t);
			}

			Gizmos.color = Color.blue;

			Gizmos.DrawWireCube(position + Vector3.up * heightRange / 2, new Vector3(xSize, heightRange, zSize));
		}

		/// <summary>
		/// 判断当前传入的某个点的位置是否存在阻挡
		///    原理 -- 当前点坐标向上的某一点发出一条射线，如果在一定高度内检测到碰撞，则判断当前点为阻挡点
		/// </summary>
		public static bool IsObstacle(float beginx, float beginy, float deltax, float deltay, float heightRange, int x, int y)
		{
			float px = beginx + x * deltax + deltax * 0.5f;
			float py = beginy + y * deltay + deltay * 0.5f;
			Ray ray = new Ray(new Vector3(px, beginy + heightRange, py), Vector3.down);
			if (Physics.Raycast(ray, heightRange))
			{
				return true;
			}
			return false;
		}
	}
}
