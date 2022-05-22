using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Fow.core.calcluater.entity
{
	public interface IFowMapData
	{
		/// <summary>
		/// 生成地图数据
		/// </summary>
		void GenerateMapData(float beginx, float beginy, float deltax, float deltay, float heightRange);

		/// <summary>
		/// 判断当前点是否为阻挡点
		/// </summary>
		FowMapPos this[int i, int j] { get; }
	}
}
