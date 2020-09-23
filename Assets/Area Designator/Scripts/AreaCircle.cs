using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Area))]
public class AreaCircle : MonoBehaviour
{
	private Area Area;

	[Min(0.1f)] public float Radius = 1;
	[Min(3)] public int NoOfPoints = 8;

	private void OnValidate()
	{
		Area = Area ?? GetComponent<Area>();

		float Theta = (2 * Mathf.PI) / NoOfPoints;
		Area.PerimeterPoints.Clear();
		for (int i = 0; i < NoOfPoints; i++)
		{
			Vector3 P = Radius * new Vector3(Mathf.Cos(i * Theta), 0, Mathf.Sin(i * Theta));
			Area.PerimeterPoints.Add(P);
		}

		Area.GetComponents<IAreaAutoUpdate>().ToList().ForEach(A => A.AreaUpdate());
	}
}
