using UnityEngine;

namespace VectorExtensions
{
    public enum Coordinate2D
    {
        X,
        Y
    }
    public enum Coordinate3D
    {
        X = Coordinate2D.X,
        Y = Coordinate2D.Y,
        Z
    }

    public static class VectorExtensions
    {
        public static Vector3 Invert(this Vector3 vector, Coordinate3D coordinate)
        {
            switch(coordinate)
            {
                case Coordinate3D.X:
                    return new Vector3(-vector.x, vector.y, vector.z);
                case Coordinate3D.Y:
                    return new Vector3(vector.x, -vector.y, vector.z);
                case Coordinate3D.Z:
                    return new Vector3(vector.x, vector.y, -vector.z);
                default:
                    throw new System.NotImplementedException();
            }
        }
        public static Vector2 Invert(this Vector2 vector, Coordinate2D coordinate) =>
            vector.ToV3().Invert((Coordinate3D)coordinate).XY();
        public static Vector3Int Invert(this Vector3Int vector, Coordinate3D coordinate) =>
            vector.ToFloat().Invert(coordinate).ToInt();
        public static Vector2Int Invert(this Vector2Int vector, Coordinate2D coordinate) =>
            vector.ToFloat().Invert(coordinate).ToInt();

        public static Vector3 Mply(this Vector3Int vector, float value) =>
            vector.ToFloat() * value;
        public static Vector2 Mply(this Vector2Int vector, float value) =>
            vector.ToFloat() * value;

        public static Vector2 Swap(this Vector2 vector) =>
            new Vector2(vector.y, vector.x);


        private static Vector2Int ToInt(this Vector2 vector) =>
            new Vector2Int((int)vector.x, (int)vector.y);
        private static Vector3Int ToInt(this Vector3 vector) =>
            new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);

        public static Vector2 ToFloat(this Vector2Int vector) =>
            new Vector2(vector.x, vector.y);
        public static Vector3 ToFloat(this Vector3Int vector) =>
            new Vector3(vector.x, vector.y, vector.z);

        public static Vector3Int ToV3(this Vector2Int vector, float z = 0) =>
            vector.ToFloat().ToV3(z).ToInt();

        public static Vector3Int ToV3(this Vector2Int vector,
            Coordinate3D coordinateToAdd, float coordinateToAddValue = 0) =>
            vector.ToFloat().ToV3(coordinateToAdd, coordinateToAddValue).ToInt();


        public static Vector3 ToV3(this Vector2 vector, float z = 0) =>
            vector.ToV3(Coordinate3D.Z, z);

        public static Vector3 ToV3(this Vector2 vector,
            Coordinate3D coordinateToAdd, float coordinateToAddValue = 0)
        {
            switch(coordinateToAdd)
            {
                case Coordinate3D.X:
                    return new Vector3(coordinateToAddValue, vector.x, vector.y);
                case Coordinate3D.Y:
                    return new Vector3(vector.x, coordinateToAddValue, vector.y);
                case Coordinate3D.Z:
                    return new Vector3(vector.x, vector.y, coordinateToAddValue);
                default:
                    throw new System.NotImplementedException();
            }
        }

        public static Vector2 XY(this Vector3 vector) =>
            new Vector2(vector.x, vector.y);
        public static Vector2 XZ(this Vector3 vector) =>
            new Vector2(vector.x, vector.z);
        public static Vector2 YX(this Vector3 vector) =>
            new Vector2(vector.y, vector.x);
        public static Vector2 YZ(this Vector3 vector) =>
            new Vector2(vector.y, vector.z);
        public static Vector2 ZX(this Vector3 vector) =>
            new Vector2(vector.z, vector.x);
        public static Vector2 ZY(this Vector3 vector) =>
            new Vector2(vector.z, vector.y);

    }
}
