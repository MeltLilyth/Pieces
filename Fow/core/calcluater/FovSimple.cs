using Assets.Scripts.Fow.core.calcluater.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fow.core.calcluater
{
	internal class FovSimple : FovCalculator
	{
		protected override void RayCast(FowMapPos pos, int centX, int centZ, float radius)
		{
			float r = radius * base.invDelta_x;
			
			Vector2 dir = new Vector2(pos.x - centX, pos.y - centZ);
			float l = dir.magnitude;
			
			if (r - l <= 0) { return;  }
			
			dir = dir.normalized * (r - l);

			int x = pos.x + (int)dir.x;
			int y = pos.y + (int)dir.y;

			base.SetInVisibleInline(pos.x, pos.y, x, y, mapData[centX, centZ] , radius * radius);
		}
	}
}
