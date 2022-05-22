using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fow.core.calcluater.entity
{
	public class FowFieldData
	{
		public float radiusSquare;
		public float radius;
		public Vector3 position;

		public FowFieldData(float radius, Vector3 position)
		{
			this.radius = radius;
			this.radius = radius * radius;
			this.position = position;
		}
	}
}
