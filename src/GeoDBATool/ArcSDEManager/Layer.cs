
using System;
using System.Runtime.InteropServices;

namespace GeoDBATool
{
	/// <summary>
	/// Layer ��ժҪ˵����
	/// </summary>
	public class Layer
	{
		#region API����

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
			public struct SE_TIME
		{
			public Int32 tm_sec;     /* seconds after the minute - [0,59] */
			public Int32 tm_min;     /* minutes after the hour - [0,59] */
			public Int32 tm_hour;    /* hours since midnight - [0,23] */
			public Int32 tm_mday;    /* day of the month - [1,31] */
			public Int32 tm_mon;     /* months since January - [0,11] */
			public Int32 tm_year;    /* years since 1900 */
			public Int32 tm_wday;    /* days since Sunday - [0,6] */
			public Int32 tm_yday;    /* days since January 1 - [0,365] */
			public Int32 tm_isdst;   /* daylight savings time flag */
		}

		/// <summary>
		/// ��ȡ��ǰ���ӵ�ͼ����Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layer_get_info_list( IntPtr pConn, out IntPtr layer_list, out Int32 layer_count );

		/// <summary>
		/// �ͷŵ�ǰ���ӵ�ͼ����Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern void SE_layer_free_info_list( Int32 layer_count, IntPtr layer_list );

		/// <summary>
		/// ��ȡָ��ͼ���ID
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layerinfo_get_id( IntPtr layer_info, out Int32 layer_id );

		/// <summary>
		/// ��ȡָ��ͼ��ķ���Ȩ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layerinfo_get_access( IntPtr layer_info, out Int32 privileges );

		/// <summary>
		/// ��ȡָ��ͼ��Ĵ���ʱ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layerinfo_get_creation_date( IntPtr layer_info, ref SE_TIME ctime );

		/// <summary>
		/// ��ȡָ��ͼ��Ŀռ伸������
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layerinfo_get_shape_types( IntPtr layer_info, out Int32 shape_types );

		/// <summary>
		/// ��ȡָ��ͼ������ƺͼ����ֶ���
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layerinfo_get_spatial_column(IntPtr layerinfo, [Out, MarshalAs(UnmanagedType.LPArray, 
					SizeConst = 1808)]char[] layername, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 256)]char[] column);

		/// <summary>
		/// ��ȡָ��ͼ��Ĵ洢����
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_layerinfo_get_storage_type( IntPtr layer_info, out Int32 storage_type );

		/// <summary>
		/// ָ��ͼ���Ƿ���ڿռ�����
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern bool SE_layerinfo_has_spatial_index( IntPtr layer_info );

        /// <summary>
        /// ��ȡͼ��Ŀռ�ο�
        /// </summary>
        [DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern Int32 SE_layerinfo_get_coordref(IntPtr layer_info, out IntPtr coordref);

        /// <summary>
        /// ��ȡͼ��Ŀռ���������
        /// </summary>
        [DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern Int32 SE_coordref_get_description(IntPtr coordref, [Out, MarshalAs(UnmanagedType.LPArray,
                      SizeConst = 1808)]char[] coordescription);

        /// <summary>
        /// ��ȡͼ������
        /// </summary>
        [DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
        public static extern Int32 SE_layerinfo_get_description(IntPtr layer_info, [Out, MarshalAs(UnmanagedType.LPArray,
                    SizeConst = 1808)]char[] description);

		#endregion
	}
}
