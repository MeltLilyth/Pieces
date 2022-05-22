using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Fow.core.calcluater.entity
{
	public class FowMapPos
	{
		public int x { private set; get; }


		public int y { private set; get; }

		/// <summary>
		/// 当前点的坐标数据是否为地图中的阻挡点
		/// </summary>
		public bool isMapBlock { private set; get; }

		/// <summary>
		/// 是否已经经过检测
		/// </summary>
		private bool isCanSee = false;

		/// <summary>
		/// 检测次数
		/// </summary>
		private short checkTime = 0;

		/// <summary>
		/// 遍历当前点是否可见的对象唯一Id
		/// </summary>
		private short checkInstanceId = -1;

		public FowMapPos(int x, int y, bool isMapBlock) {
			this.x = x;
			this.y = y;
			this.isMapBlock = isMapBlock;

			this.isCanSee = false;
			this.checkTime = 0;
			this.checkInstanceId = -1;
		}

		/// <summary>
		/// 设定当前点的坐标为可见点数据
		/// </summary>
		/// <param name="checkTime"></param>
		public void SetFowMapPosVisiable(short checkTime, short instanceId) {
			this.isCanSee = true;
			this.SetFowMapPosCheckTime(checkTime);
			this.checkInstanceId = instanceId;
		}

		public void SetFowMapPosCheckTime(short checkTime) { 
			this.checkTime = checkTime; 
		}

		/// <summary>
		/// 在指定的检测次数中，当前的检测点是否是可见点
		/// </summary>
		/// <param name="checkTime"></param>
		/// <returns></returns>
		public bool IsFowMapPosCheckedAndSee(short checkTime) {
			if (isMapBlock) { 
				return false; 
			}

			if (isCanSee && this.checkTime == checkTime) {
				return true;
			}
			else{
				if (isCanSee) { this.isCanSee = false; }
				return false;
			}
		}

		/// <summary>
		/// 判断当前点是否需要再次检测 true 为需要再次检测
		/// </summary>
		public bool IsMapPosShouldChcekAgain(short instanceId, short checkTime, bool isMapBlockRelease = false) {
			//同一个对象，判断是不是检测的波数是否为当前波数
			if (this.checkInstanceId == instanceId) { return this.checkTime != checkTime; }

			//如果为不同的对象，且同时处于同一个检测时段，并且检测出的对象是不可见点相关的点，如果已经被判断为可见点位置，就不需要检测
			if (this.checkTime == checkTime && isMapBlockRelease){
				return !isCanSee;
			}

			return true;
		}

	}
}
