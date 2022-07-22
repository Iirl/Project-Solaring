using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using System;
using System.Runtime.InteropServices;



public enum MetaballSources
{
    ChildMetaballs, ParticleSystem
}

public enum RegenSchedule
{
    EveryFrame, EveryFrameIfFixedUpdate, OnDemandOnly
}

[AddComponentMenu("DX11Metaballs/MetaballController")]
[ExecuteInEditMode]
public class MetaballController : MonoBehaviour {

    //-----------------------
    // Serialized properties
    //-----------------------
    [Range(0.1f, 1.0f)]
    public float isoValue = 0.5f;
    public Vector3 cellSize = new Vector3(1.0f, 1.0f, 1.0f);
    public MetaballSources sourceType;
    public ParticleSystem sourceParticleSystem;


    public int MaximumBalls = 1024;
    public int maxAttribElems = 800000;
    public int maxIndexElems = 3 * 800000;
    public int hashBufferGridBuckets = 2500;
    public int hashBufferGridElemsPerBucket = 32;
    public int hashBufferBallBuckets = 2500;

    public Material material;
    public bool castShadows = true;
    public bool receiveShadows = true;
    public RegenSchedule regenSchedule;

    //---------------
    // C# Properties
    //---------------
    public Vector3 voxelSize { get { return Vector3.Scale(cellSize, MetaballUtil.CellsPerVoxel); } }
    public int MaxGridResults { get { return hashBufferGridBuckets * hashBufferGridElemsPerBucket; } }
    public int BallBucketInts { get { return (MaximumBalls / 32) + ((MaximumBalls % 32) > 0 ? 1 : 0); } }

    //----------
    // Shaders
    //----------
    //Resets hash Buffers
    private static ComputeShader resetShader;
    //Spatially hashes input balls producing 2 hashtables ( ballHashBuffer, gridHashBuffer)
    private static ComputeShader hashShader;
    //Reduces gridHashBuffer to set of voxels with at least one overlapping ball
    private static ComputeShader ballCompactShader;
    //Reduces compacted Voxels to set of voxels which the isosurface intersects
    private static ComputeShader surfaceCompactShader;
    // Produces indexed triangle mesh of isosurface
    private static ComputeShader meshShader;


    //---------
    // Buffers
    //---------
    //Hashtable mapping voxelID's to list of voxels with same hash :: int3 -> [int3]
    private ComputeBuffer gridHashBuffer;
    //Hashtable mapping voxelID's to list of balls overlapping voxel :: int3 -> [ballID] (ballID := Int) 
    private ComputeBuffer ballHashBuffer;
    //List of voxelID's which overlap at least one ball
    private ComputeBuffer ballCompactBuffer;
    //List of voxelId's which intersect isoSurface
    private ComputeBuffer surfaceCompactBuffer;
    //Total number of voxels intersected by balls
    private ComputeBuffer voxelSumBuffer;
    //Total number of voxels intersected by isosurface
    private ComputeBuffer surfaceSumBuffer;
    //Controls triangulation
    private ComputeBuffer triCaseBuffer;

    //----------
    // Privates
    //----------
    private Bounds ballBounds;
    private Vector4[] ballData;
    private ParticleSystem.Particle[] particles;
    private Mesh dummyMesh;
    private System.IntPtr dummyAttribPtr, dummyIndexPtr, bufferAttribPtr, bufferIndexPtr;
    private ComputeBuffer ballBuffer, attribBuffer, indexBuffer, sumBuffer;
    private bool meshNeedsRegen = false;
    [SerializeField]
    private bool needsReInit = false;



    //----------------
    // Native Imports
    //----------------
    [DllImport("NativeBufferCopy")]
    static extern System.IntPtr GetCopyCallback();

    //---------------
    //Initialization
    //---------------
    private void Init()
    {
        LoadShaders();
        InitSourceData();
        InitBuffers();
        InitDummyMesh();
    }
    private void ReInit()
    {
        DisposeOfBuffers();
        Init();
    }
    private void LoadShaders()
    {
        resetShader = resetShader ?? TryLoadComputeShader("ResetHashBuffers.compute");
        hashShader = hashShader ?? TryLoadComputeShader("SpatialHash.compute");
        ballCompactShader = ballCompactShader ?? TryLoadComputeShader("CompactByBalls.compute");
        surfaceCompactShader = surfaceCompactShader ?? TryLoadComputeShader("CompactBySurface.compute");
        meshShader = meshShader ?? TryLoadComputeShader("GenerateMesh.compute");
    }
    private void InitSourceData()
    {
        particles = new ParticleSystem.Particle[MaximumBalls];
        ballData = new Vector4[MaximumBalls];
        ballBounds = new Bounds(Vector3.zero, Vector3.one);
    }
    private void InitDummyMesh()
    {
        dummyMesh = new Mesh();
        dummyMesh.indexFormat = IndexFormat.UInt32;

        var l = new Vector3[maxAttribElems];
        var n = new int[maxIndexElems];
        dummyMesh.vertices = l;
        dummyMesh.normals = l;
        dummyMesh.SetIndices(n, MeshTopology.Triangles, 0);
        dummyAttribPtr = dummyMesh.GetNativeVertexBufferPtr(0);
        dummyIndexPtr = dummyMesh.GetNativeIndexBufferPtr();
    }
    private void InitBuffers()
    {
        surfaceSumBuffer = new ComputeBuffer(3, sizeof(uint), ComputeBufferType.IndirectArguments);
        surfaceSumBuffer.SetData(new int[] { 0, 1, 1 });
        voxelSumBuffer = new ComputeBuffer(3, sizeof(uint), ComputeBufferType.IndirectArguments);
        voxelSumBuffer.SetData(new int[] { 0, 1, 1 });
        triCaseBuffer = new ComputeBuffer(256 * 16, sizeof(uint), ComputeBufferType.Default);
        triCaseBuffer.SetData(TriTable.triTable);
        gridHashBuffer = new ComputeBuffer(hashBufferGridBuckets * (hashBufferGridElemsPerBucket + MetaballUtil.CountReserved), MetaballUtil.VoxelByteSize);
        ballCompactBuffer = new ComputeBuffer(MaxGridResults, MetaballUtil.VoxelByteSize);
        surfaceCompactBuffer = new ComputeBuffer(MaxGridResults, MetaballUtil.VoxelByteSize);
        ballHashBuffer = new ComputeBuffer(hashBufferBallBuckets * (BallBucketInts + MetaballUtil.CountReserved), MetaballUtil.BallIDByteSize);
        ballBuffer = new ComputeBuffer(MaximumBalls, MetaballUtil.BallStride, ComputeBufferType.Default);
        attribBuffer = new ComputeBuffer(maxAttribElems, MetaballUtil.AttribStride, ComputeBufferType.Default);
        bufferAttribPtr = attribBuffer.GetNativeBufferPtr();
        indexBuffer = new ComputeBuffer(maxIndexElems, MetaballUtil.IndexStride, ComputeBufferType.Default);
        bufferIndexPtr = indexBuffer.GetNativeBufferPtr();
        sumBuffer = new ComputeBuffer(6, sizeof(int), ComputeBufferType.IndirectArguments);
        sumBuffer.SetData(new int[] { 0, 0, 1, 0, 0, 0 });
    }


    //----------------
    // Ball Data
    //----------------
    private void FillData(out int numBalls, out Bounds ballBounds)
    {
        switch (sourceType)
        {
            case MetaballSources.ChildMetaballs:
                MBallFill(out numBalls, out ballBounds);
                break;
            case MetaballSources.ParticleSystem:
                ParticleFill(out numBalls, out ballBounds);
                break;
            default:
                EmptyFill(out numBalls, out ballBounds);
                break;

        }
    }
    private void EmptyFill(out int numBalls, out Bounds ballBounds)
    {
        numBalls = 0;
        ballBounds = new Bounds(Vector3.zero, Vector3.zero);
    }
    private void MBallFill(out int numBalls, out Bounds ballBounds)
    {
        var balls = gameObject.GetComponentsInChildren<Metaball>();
        var maxCopy = System.Math.Min(MaximumBalls, balls.Length);
        Bounds b = new Bounds();
        for (int i = 0; i < maxCopy; i++)
        {
            ballData[i] = balls[i].pack();
            b.Encapsulate(balls[i].transform.position);

        }
        ballBounds = b;
        numBalls =  balls.Length;
        ballBuffer.SetData(ballData);

    }
    private void ParticleFill(out int numBalls, out Bounds ballBounds)
    {
        if (sourceParticleSystem == null)
        {
            EmptyFill(out numBalls, out ballBounds);
            return;
        }
        Bounds b = new Bounds();
        sourceParticleSystem.GetParticles(particles);
        for (int i = 0; i < sourceParticleSystem.particleCount; i++)
        {
            Vector3 pos = particles[i].position;
            float radius = particles[i].GetCurrentSize(sourceParticleSystem) / 2.0f;
            ballData[i] = new Vector4(pos.x, pos.y, pos.z, radius);
            b.Encapsulate(pos);
        }
        ballBounds = b;
        numBalls = sourceParticleSystem.particleCount;
        ballBuffer.SetData(ballData);
    }

    //---------------------
    // LifeCycle Callbacks
    //---------------------
    public void OnEnable()
    {
        AssemblyReloadEvents.beforeAssemblyReload += DisposeOfBuffers;
        Init();
    }
    public void OnDisable()
    {
        DisposeOfBuffers();
        AssemblyReloadEvents.beforeAssemblyReload -= DisposeOfBuffers;
    }
    public void FixedUpdate()
    {
        if (regenSchedule == RegenSchedule.EveryFrameIfFixedUpdate)
        {
            meshNeedsRegen = true;
        }
    }
    public void Update()
    {
        if (regenSchedule == RegenSchedule.EveryFrame)
        {
            meshNeedsRegen = true;
        }

        if (meshNeedsRegen || (Application.isEditor && !Application.isPlaying))
        {
            int numBalls;
            FillData(out numBalls,out ballBounds);
            Generate(numBalls);
            meshNeedsRegen = false;
        }

        Render();
    }
    public void OnValidate()
    {
        cellSize.x = cellSize.x <= 0 ? 1.0f : cellSize.x;
        cellSize.y = cellSize.y <= 0 ? 1.0f : cellSize.y;
        cellSize.z = cellSize.z <= 0 ? 1.0f : cellSize.z;

        if (needsReInit)
        {
            ReInit();
            needsReInit = false;
        }
    }


    //---------------------
    // Mesh Generation
    //---------------------
    private void Generate(int numBalls)
    {
        if (numBalls > 0)
        {
            CommandBuffer buf = new CommandBuffer();
            buf.name = "Metaballs Generation Buffer";

            resetShader.SetBuffer(0, "gridHashBuffer", gridHashBuffer);
            resetShader.SetBuffer(0, "ballHashBuffer", ballHashBuffer);
            resetShader.SetBuffer(0, "surfaceSumBuffer", surfaceSumBuffer);
            resetShader.SetBuffer(0, "voxelSumBuffer", voxelSumBuffer);
            resetShader.SetBuffer(0, "sumBuffer", sumBuffer);
            resetShader.SetInt("hashBufferGridBuckets", hashBufferGridBuckets);
            resetShader.SetInt("hashBufferBallBuckets", hashBufferBallBuckets);
            resetShader.SetInt("hashBufferGridElemsPerBucket", hashBufferGridElemsPerBucket);
            resetShader.SetInt("hashBufferBallElemsPerBucket", BallBucketInts);
            resetShader.Dispatch(0, GroupsNeeded(System.Math.Max(hashBufferGridBuckets, hashBufferBallBuckets), MetaballUtil.ResetShaderNumThreads), 1, 1);

            hashShader.SetVector("voxelSize", voxelSize);
            hashShader.SetBuffer(0, "ballBuffer", ballBuffer);
            hashShader.SetBuffer(0, "gridHashBuffer", gridHashBuffer);
            hashShader.SetBuffer(0, "ballHashBuffer", ballHashBuffer);
            hashShader.SetInt("numBalls", numBalls);
            hashShader.SetInt("hashBufferGridBuckets", hashBufferGridBuckets);
            hashShader.SetInt("hashBufferBallBuckets", hashBufferBallBuckets);
            hashShader.SetInt("hashBufferGridElemsPerBucket", hashBufferGridElemsPerBucket);
            hashShader.SetInt("hashBufferBallElemsPerBucket", BallBucketInts);
            hashShader.Dispatch(0, GroupsNeeded(numBalls, MetaballUtil.HashShaderNumThreads), 1, 1);


            ballCompactShader.SetBuffer(0, "gridHashBuffer", gridHashBuffer);
            ballCompactShader.SetBuffer(0, "ballCompactBuffer", ballCompactBuffer);
            ballCompactShader.SetBuffer(0, "voxelSumBuffer", voxelSumBuffer);
            ballCompactShader.SetInt("hashBufferGridElemsPerBucket", hashBufferGridElemsPerBucket);
            ballCompactShader.Dispatch(0, GroupsNeeded(hashBufferGridBuckets, MetaballUtil.BallCompactShaderNumThreads), 1, 1);


            surfaceCompactShader.SetBuffer(0, "ballCompactBuffer", ballCompactBuffer);
            surfaceCompactShader.SetBuffer(0, "ballHashBuffer", ballHashBuffer);
            surfaceCompactShader.SetBuffer(0, "ballBuffer", ballBuffer);
            surfaceCompactShader.SetBuffer(0, "surfaceCompactBuffer", surfaceCompactBuffer);
            surfaceCompactShader.SetBuffer(0, "surfaceSumBuffer", surfaceSumBuffer);
            surfaceCompactShader.SetFloat("isoValue", isoValue);
            surfaceCompactShader.SetVector("cellSize", cellSize);
            surfaceCompactShader.SetInt("hashBufferBallBuckets", hashBufferBallBuckets);
            surfaceCompactShader.SetInt("hashBufferBallElemsPerBucket", BallBucketInts);
            surfaceCompactShader.DispatchIndirect(0, voxelSumBuffer);

            meshShader.SetVector("cellSize", cellSize);
            meshShader.SetVector("boundsCenter", ballBounds.center);
            meshShader.SetVector("boundsExtent", ballBounds.extents);
            meshShader.SetBuffer(0, "ballHashBuffer", ballHashBuffer);
            meshShader.SetBuffer(0, "surfaceCompactBuffer", surfaceCompactBuffer);
            meshShader.SetBuffer(0, "triCaseBuffer", triCaseBuffer);
            meshShader.SetBuffer(0, "ballBuffer", ballBuffer);
            meshShader.SetBuffer(0, "attribBuffer", attribBuffer);
            meshShader.SetBuffer(0, "indexBuffer", indexBuffer);
            meshShader.SetBuffer(0, "sumBuffer", sumBuffer);
            meshShader.SetInt("numBalls", numBalls);
            meshShader.SetFloat("isoValue", isoValue);
            meshShader.SetInt("hashBufferBallBuckets", hashBufferBallBuckets);
            meshShader.SetInt("hashBufferBallElemsPerBucket", BallBucketInts);
            meshShader.DispatchIndirect(0, surfaceSumBuffer, 0);


        }
    }

    //---------------------
    // Buffer Copy
    //---------------------
    private void CopyBuffers()
    {
        COPYSTRUCT csVert = new COPYSTRUCT(bufferAttribPtr, dummyAttribPtr, (uint)maxAttribElems, MetaballUtil.AttribStride);
        COPYSTRUCT csIdx = new COPYSTRUCT(bufferIndexPtr, dummyIndexPtr, (uint)maxIndexElems, MetaballUtil.IndexStride);

        //Freed in DLL
        System.IntPtr csVertPtr = Marshal.AllocHGlobal(Marshal.SizeOf(csVert));
        System.IntPtr csIdxPtr = Marshal.AllocHGlobal(Marshal.SizeOf(csIdx));

        Marshal.StructureToPtr(csVert, csVertPtr, false);
        Marshal.StructureToPtr(csIdx, csIdxPtr, false);

        //Overwrite Mesh Index/Attrib buffers
        CommandBuffer buf = new CommandBuffer();
        buf.IssuePluginEventAndData(GetCopyCallback(), 0, csVertPtr);
        buf.IssuePluginEventAndData(GetCopyCallback(), 0, csIdxPtr);
        Graphics.ExecuteCommandBuffer(buf);
    }


    //--------------------
    // Rendering
    //--------------------
    public void Render()
    {
        CopyBuffers();
        if (material != null)
        {
            Graphics.DrawMeshInstancedIndirect(dummyMesh, 0, material, ballBounds, sumBuffer, 4, null, castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off, receiveShadows);
        }
    }


    //---------
    // Statics
    //---------
    private static void SafeDispose(ComputeBuffer buf)
    {
        if (buf != null)
        {
            buf.Dispose();
        }
    }
    private static int GroupsNeeded(int iters, int groupSize)
    {
        return (iters / groupSize) + ((iters % groupSize) > 0 ? 1 : 0);
    }
    private static ComputeShader TryLoadComputeShader(string name)
    {
        var computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(MetaballUtil.ComputeShaderBasePath + name);
        if (computeShader == null)
        {
            Debug.LogError("Failed to Load required Metaball shader at " + (MetaballUtil.ComputeShaderBasePath + name) + ". If you haved move the folder elsewhere please edit the variable ComputeShaderBasePath in MetaballUtil.cs");
        }
        return computeShader;
    }
    public void DisposeOfBuffers()
    {
        SafeDispose(sumBuffer);
        SafeDispose(attribBuffer);
        SafeDispose(indexBuffer);
        SafeDispose(ballBuffer);
        SafeDispose(triCaseBuffer);
        SafeDispose(gridHashBuffer);
        SafeDispose(ballHashBuffer);
        SafeDispose(ballCompactBuffer);
        SafeDispose(surfaceCompactBuffer);
        SafeDispose(surfaceSumBuffer);
        SafeDispose(voxelSumBuffer);
    }
}

[CustomEditor(typeof(MetaballController))]
public class MetaballControllerEditor : Editor
{
    private bool advancedFoldOut = true;
    int maxBallsTemp = 0;
    int maxAttribElemsTemp = 0;
    int maxIndexElemsTemp = 0;
    int hashBufferGridBucketsTemp = 0;
    int hashBufferGridElemsPerBucketTemp = 0;
    int hashBufferBallBucketsTemp = 0;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isoValue"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cellSize"));
        var sourceTypeProp = serializedObject.FindProperty("sourceType");
        EditorGUILayout.PropertyField(sourceTypeProp);
        serializedObject.ApplyModifiedProperties();
        if (sourceTypeProp.enumValueIndex == (int)MetaballSources.ParticleSystem)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sourceParticleSystem"));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("regenSchedule"));
        var materialProp = serializedObject.FindProperty("material");
        EditorGUILayout.PropertyField(materialProp);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("castShadows"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("receiveShadows"));

        advancedFoldOut = EditorGUILayout.Foldout(advancedFoldOut, "Advanced", true);
        if(advancedFoldOut)
        {
            EditorGUI.indentLevel++;
            showAdvancedSection();
            EditorGUI.indentLevel--;
        }
        

        if (materialProp.objectReferenceValue != null)
        {
            showMaterialInspector(materialProp);
        }
        serializedObject.ApplyModifiedProperties();

    }

    private void showAdvancedSection()
    {
        var maxBallsProp = serializedObject.FindProperty("MaximumBalls");
        var maxAttribElemsProp = serializedObject.FindProperty("maxAttribElems");
        var maxIndexElemsProp = serializedObject.FindProperty("maxIndexElems");
        var hashBufferGridBucketsProp = serializedObject.FindProperty("hashBufferGridBuckets");
        var hashBufferGridElemsPerBucketProp = serializedObject.FindProperty("hashBufferGridElemsPerBucket");
        var hashBufferBallBucketsProp = serializedObject.FindProperty("hashBufferBallBuckets");

        maxBallsTemp = maxBallsTemp == 0 ? maxBallsProp.intValue : maxBallsTemp;
        maxAttribElemsTemp = maxAttribElemsTemp == 0 ? maxAttribElemsProp.intValue : maxAttribElemsTemp;
        maxIndexElemsTemp = maxIndexElemsTemp == 0 ? maxIndexElemsProp.intValue : maxIndexElemsTemp;
        hashBufferGridBucketsTemp = hashBufferGridBucketsTemp == 0 ? hashBufferGridBucketsProp.intValue : hashBufferGridBucketsTemp;
        hashBufferGridElemsPerBucketTemp = hashBufferGridElemsPerBucketTemp == 0 ? hashBufferGridElemsPerBucketProp.intValue : hashBufferGridElemsPerBucketTemp;
        hashBufferBallBucketsTemp = hashBufferBallBucketsTemp == 0 ? hashBufferBallBucketsProp.intValue : hashBufferBallBucketsTemp;

        maxBallsTemp = EditorGUILayout.IntField("Maximum Balls", maxBallsTemp);
        maxAttribElemsTemp = EditorGUILayout.IntField("Max Output Verts", maxAttribElemsTemp);
        maxIndexElemsTemp = EditorGUILayout.IntField("Max Output Indices", maxIndexElemsTemp);
        hashBufferGridBucketsTemp = EditorGUILayout.IntField("Grid Hash Table Buckets", hashBufferGridBucketsTemp);
        hashBufferGridElemsPerBucketTemp = EditorGUILayout.IntField("Grid Hash Table Elements Per Bucket", hashBufferGridElemsPerBucketTemp);
        hashBufferBallBucketsTemp = EditorGUILayout.IntField("Ball Hash Table Buckets", hashBufferBallBucketsTemp);

        if (GUILayout.Button("Apply"))
        {
            maxBallsProp.intValue = maxBallsTemp;
            maxAttribElemsProp.intValue = maxAttribElemsTemp;
            maxIndexElemsProp.intValue = maxIndexElemsTemp;
            hashBufferGridBucketsProp.intValue = hashBufferGridBucketsTemp;
            hashBufferGridElemsPerBucketProp.intValue = hashBufferGridElemsPerBucketTemp;
            hashBufferBallBucketsProp.intValue = hashBufferBallBucketsTemp;
            serializedObject.FindProperty("needsReInit").boolValue = true;
            serializedObject.ApplyModifiedProperties();

        }
    }
    private void showMaterialInspector(SerializedProperty materialProp)
    {
        MaterialEditor _matEditor = null;

        _matEditor = (MaterialEditor)Editor.CreateEditor((Material)materialProp.objectReferenceValue);
        _matEditor.DrawHeader();
        bool isDefaultMaterial = !AssetDatabase.GetAssetPath((Material)materialProp.objectReferenceValue).StartsWith("Asset");

        using (new EditorGUI.DisabledGroupScope(isDefaultMaterial))
        {
            _matEditor.OnInspectorGUI();
        }
        Editor.DestroyImmediate(_matEditor);
    }
}

