using Smod2;
using System.Threading;

namespace SpyCup
{
	class SetRotation
	{
		public SetRotation(Plugin plugin, UnityEngine.GameObject pObj, float rotation)
		{
			Thread.Sleep(100);
			pObj.GetComponent<PlyMovementSync>().SetRotation(rotation - pObj.GetComponent<PlyMovementSync>().rotation);
		}
	}
}
