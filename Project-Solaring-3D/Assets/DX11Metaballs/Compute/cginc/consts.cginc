
static const uint4 edgeNeighbourTable[12] = {
    uint4(0, 0, 0, 0), // xyz for neighbour voxel offset. w = 0 for x edge, 1 for y edge, 2 for z edge.
    uint4(1, 0, 0, 2),
    uint4(0, 0, 1, 0),
    uint4(0, 0, 0, 2),
    uint4(0, 1, 0, 0),
    uint4(1, 1, 0, 2),
    uint4(0, 1, 1, 0),
    uint4(0, 1, 0, 2),
    uint4(0, 0, 0, 1),
    uint4(1, 0, 0, 1),
    uint4(1, 0, 1, 1),
    uint4(0, 0, 1, 1)
};

static const uint3 cubeVerts[8] =
{
    uint3(0, 0, 0),
    uint3(1, 0, 0),
    uint3(1, 0, 1),
    uint3(0, 0, 1),
    uint3(0, 1, 0),
    uint3(1, 1, 0),
    uint3(1, 1, 1),
    uint3(0, 1, 1)
};