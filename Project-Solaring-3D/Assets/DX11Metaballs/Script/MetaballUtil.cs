using UnityEngine;

    public static class MetaballUtil
    {
        public const int BallIDByteSize = sizeof(uint);
        public const int VoxelByteSize = sizeof(int) * 3;
        public const int BallStride = sizeof(float) * 4;
        public const int NormalStride = sizeof(float) * 3;
        public const int PositionStride = sizeof(float) * 3;
        public const int AttribStride = NormalStride + PositionStride;
        public const int IndexStride = sizeof(uint);
        public const int CountReserved = 1;
        public static Vector3Int CellsPerVoxel = new Vector3Int(3, 3, 7);
        public static Bounds MaxBounds = new Bounds(Vector3.zero, new Vector3(1000.0f, 1000.0f, 1000.0f));

        public const int ResetShaderNumThreads = 512;
        public const int HashShaderNumThreads = 512;
        public const int BallCompactShaderNumThreads = 128;
        public const string ComputeShaderBasePath = "Assets/DX11Metaballs/compute/";


    }

