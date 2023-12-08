using System.Numerics;

namespace Sedulous.Mathematics;

//TODO: See if this can just be replaced with a call to Vector3.Lerp
/// <summary>
/// An implementation of the <see cref="IInterpolatable{Vector3}"/> interface that works with instances of the <see cref="Vector3"/> type.
/// </summary>
internal class Vector3Interpolatable : IInterpolatable<Vector3> {
	/// <summary>
	/// The <see cref="Vector3"/> value that will be used as the "minimum" value of the interpolation.
	/// </summary>
	public Vector3 Vector { get; set; }

	Vector3 IInterpolatable<Vector3>.Interpolate(Vector3 target, float t) {
		Vector3 result = new();

		result.X = Tweening.Lerp(Vector.X, target.X, t);
		result.Y = Tweening.Lerp(Vector.Y, target.Y, t);
		result.Z = Tweening.Lerp(Vector.Z, target.Z, t);

		return result;
	}

	/// <summary>
	/// Interpolates between the two <see cref="Vector3"/> values.
	/// </summary>
	/// <param name="minimum"></param>
	/// <param name="maximum"></param>
	/// <param name="t"></param>
	/// <returns></returns>
	public static Vector3 Interpolate(Vector3 minimum, Vector3 maximum, float t) {
		Vector3 result = new();

		result.X = Tweening.Lerp(minimum.X, maximum.X, t);
		result.Y = Tweening.Lerp(minimum.Y, maximum.Y, t);
		result.Z = Tweening.Lerp(minimum.Z, maximum.Z, t);

		return result;
	}
}
