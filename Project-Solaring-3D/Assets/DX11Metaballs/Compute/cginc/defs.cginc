
#define BIT_CHECK(v,x) ((v >> x) & 1)
#define VOXEL_CELLS_X 3
#define VOXEL_CELLS_Y 3
#define VOXEL_CELLS_Z 7
#define VOXEL_VERTS_X (VOXEL_CELLS_X+1)
#define VOXEL_VERTS_Y (VOXEL_CELLS_Y+1)
#define VOXEL_VERTS_Z (VOXEL_CELLS_Z+1)

//Note this doesn't actually set bit n of x to v. It ONLY sets that bit to 1, if v is 1 and the bit is {0,1}. IT cannot set bit n to 0 if it is already 1
#define COND_SET_BIT(x,n,v) (x |= (v) << n)