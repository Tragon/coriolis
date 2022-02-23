using UnityEngine;

public static class HexMetrics
{
	public const float radFactor = 0.866025404f;
	public const float outerRadius = 10f;
	public const float innerRadius = outerRadius * radFactor;
	public const float scaler = 0.95f;

	public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius * scaler),
		new Vector3(innerRadius * scaler, 0f, 0.5f * outerRadius * scaler),
		new Vector3(innerRadius * scaler, 0f, -0.5f * outerRadius * scaler),
		new Vector3(0f, 0f, -outerRadius * scaler),
		new Vector3(-innerRadius * scaler, 0f, -0.5f * outerRadius * scaler),
		new Vector3(-innerRadius * scaler, 0f, 0.5f * outerRadius * scaler)
	};

	public static Vector3[] cornersShip = {
		new Vector3(0f, 0f, outerRadius ),
		new Vector3(innerRadius , 0f, 0.5f * outerRadius),
		new Vector3(innerRadius , 0f, -0.5f * outerRadius ),
		new Vector3(0f, 0f, -outerRadius ),
		new Vector3(-innerRadius , 0f, -0.5f * outerRadius ),
		new Vector3(-innerRadius , 0f, 0.5f * outerRadius )
	};

	public static Vector3[,] triangles = {
		{
			new Vector3(1f - scaler, 0f, outerRadius * scaler),
			new Vector3(innerRadius * scaler, 0f, 0.5f * outerRadius * scaler),
			new Vector3(innerRadius * scaler, 0f, -0.5f * outerRadius * scaler)
		},
		{
			new Vector3(innerRadius * scaler, 0f, 0.5f * outerRadius * scaler),
			new Vector3(innerRadius * scaler, 0f, -0.5f * outerRadius * scaler),
			new Vector3(0f, 0f, -outerRadius * scaler)
		},
		{
			new Vector3(innerRadius * scaler, 0f, -0.5f * outerRadius * scaler),
			new Vector3(0f, 0f, -outerRadius * scaler),
			new Vector3(-innerRadius * scaler, 0f, -0.5f * outerRadius * scaler)
		},
		{
			new Vector3(0f, 0f, -outerRadius * scaler),
			new Vector3(-innerRadius * scaler, 0f, -0.5f * outerRadius * scaler),
			new Vector3(-innerRadius * scaler, 0f, 0.5f * outerRadius * scaler),
		},
		{
			new Vector3(-innerRadius * scaler, 0f, -0.5f * outerRadius * scaler),
			new Vector3(-innerRadius * scaler, 0f, 0.5f * outerRadius * scaler),
			new Vector3(0f, 0f, outerRadius * scaler)
		},
		{
			new Vector3(-innerRadius * scaler, 0f, 0.5f * outerRadius * scaler),
			new Vector3(0f, 0f, outerRadius * scaler),
			new Vector3(innerRadius * scaler, 0f, 0.5f * outerRadius * scaler)
		}
	};

	public static Vector3 move(Vector3 v, int x, int z) {
		Vector3 position;
		position.x = v.x + (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = v.y;
		position.z = v.z + z * (HexMetrics.outerRadius * 1.5f);
		return position;
	}

	public static float getFactor() {
		return Mathf.Sqrt(3f) / 2f;

	}
}
