using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct COPYSTRUCT
{
    System.IntPtr from_buf;
    System.IntPtr to_buf;
    uint num_elems;
    uint elem_byte_size;

    public COPYSTRUCT(System.IntPtr from_buf, System.IntPtr to_buf, uint num_elems, uint elem_byte_size)
    {
        this.from_buf = from_buf;
        this.to_buf = to_buf;
        this.num_elems = num_elems;
        this.elem_byte_size = elem_byte_size;
    }
}