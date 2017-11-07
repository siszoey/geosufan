
using System;
using System.Runtime.InteropServices;

namespace GeoDBATool
{
	/// <summary>
	/// Version ��ժҪ˵����
	/// </summary>
	public class Version
	{
		/// <summary>
		/// ��ȡ��ǰ���ӵİ汾��Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_version_get_info_list( IntPtr pConn, string where_clause, out IntPtr version_list, out Int32 version_count );

		/// <summary>
		/// �ͷŵ�ǰ���ӵİ汾��Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern void SE_version_free_info_list( Int32 version_count, IntPtr version_list );

		/// <summary>
		/// ��ȡָ���汾��ID
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_versioninfo_get_id( IntPtr version_info, out Int32 version_id );

		/// <summary>
		/// ��ȡָ���汾������
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_versioninfo_get_name( IntPtr version_info, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 64)]char[] version_name );

		/// <summary>
		/// ��ȡָ���汾�ķ���Ȩ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_versioninfo_get_access( IntPtr version_info, out Int32 version_access );

		/// <summary>
		/// ��ȡָ���汾�ĸ��汾����
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_versioninfo_get_parent_name( IntPtr version_info, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 64)]char[] parent_name );

		/// <summary>
		/// ��ȡָ���汾�Ĵ���ʱ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_versioninfo_get_creation_time( IntPtr version_info, ref Layer.SE_TIME ctime );

		/// <summary>
		/// ��ȡָ���汾�ĸ��汾����
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_versioninfo_get_description( IntPtr version_info, [Out, MarshalAs(UnmanagedType.LPArray, SizeConst = 64)]char[] description );


	}
}
