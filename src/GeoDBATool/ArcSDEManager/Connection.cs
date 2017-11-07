
using System;
using System.Runtime.InteropServices;

namespace GeoDBATool
{
	/// <summary>
	/// Connection ��ժҪ˵����
	/// </summary>
	public class Connection
	{

		#region �ṹ����

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SE_ERROR
		{
			public Int32 sde_error;
			public Int32 ext_error;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
			public char[] err_msg1;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
			public char[] err_msg2;
		}

		#endregion

		//////////
		// API����
		///////////

		#region Connection

		/// <summary>
		/// �ύ��������SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_connection_commit_transaction( IntPtr pSdeConn );

		/// <summary>
		/// ����һ������ArcSDE����SE_CONNECTION ��
		/// </summary>
        //[DllImport(".\\sde91.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
        [DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_connection_create(string server, string instance, string database, string username, string pwd, ref SE_ERROR error, out IntPtr pSdeConn);

		/// <summary>
		/// �رյ�ǰ��ArcSDE�������ӣ�SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern void  SE_connection_free( IntPtr pSdeConn );

		/// <summary>
		/// �����ǰSDE��������������SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern void  SE_connection_free_all_locks( IntPtr pSdeConn );

		/// <summary>
		/// ��õ�ǰSDE�������ӵĹ������ݿ����ƣ�SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_connection_get_admin_database( IntPtr pSdeConn, out string admin_databases );

		/// <summary>
		/// ��õ�ǰSDE�������ӵ����ݿ����ƣ�SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_connection_get_database( IntPtr pSdeConn, out string databases );

		/// <summary>
		/// ��õ�ǰ���ݿ��DBMS��Ϣ��SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_connection_get_dbms_info( IntPtr pSdeConn, out Int32 dbms_id, out Int32 dbms_properties );

		/// <summary>
		/// ��õ�ǰSDE�����ϵͳʱ�䣨SE_CONNECTION ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_connection_get_server_time( IntPtr pSdeConn );

		
		
		#endregion

		#region Instance

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SE_INSTANCE_USER
		{
			public Int32 svrpid;
			public Int32 cstime;
			public bool xdr_needed;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
			public char[] sysname;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
			public char[] nodename;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 33)]
			public char[] username;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SE_VERSION
		{
			public Int32 major;
			public Int32 minor;
			public Int32 bug;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
			public char[] desc;
			public Int32 release;
			public Int32 rel_low_supported;
		}


		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SE_INSTANCE_STATUS
		{
			public SE_VERSION version;
			public Int32 connections;
			public Int32 system_mode;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct SE_INSTANCE_STATS
		{
			public Int32 pid;
			public Int32 rcount;
			public Int32 wcount;
			public Int32 opcount; 
			public Int32 numlocks;
			public Int32 fb_partial;
			public Int32 fb_count;
			public Int32 fb_fcount;
			public Int32 fb_kbytes;
		}

		/// <summary>
		/// ��õ�ǰ�����û�������Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_instance_get_users( string server, string instance, out IntPtr user_list, out Int32 user_count );

		/// <summary>
		/// �ͷ��û�������Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern void SE_instance_free_users( ref SE_INSTANCE_USER[] user_list, Int32 user_count );

		/// <summary>
		/// ���Ʒ�������SDE�����״̬
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_instance_control( string strServer, string strInstance, string strPwd, Int32 iOption, Int32 iPid );

		/// <summary>
		/// ��õ�ǰ��������ʵ��״̬��Ϣ
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_instance_status( string strServer, string strInstance, ref SE_INSTANCE_STATUS pStatus );

		/// <summary>
		/// ������ǰ��������ʵ��
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_instance_start( string strSdeHome, string strPwd );

		/// <summary>
		/// ��õ�ǰ����ʵ����ͳ����Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern Int32 SE_instance_get_statistics( string strServer, string strInstance, out IntPtr stats_list, out Int32 stats_count );

		/// <summary>
		/// �ͷŵ�ǰ����ʵ����ͳ����Ϣ�б�
		/// </summary>
		[DllImport(".\\sde.dll", SetLastError = true, ThrowOnUnmappableChar = true)]
		public static extern void SE_instance_free_statistics( ref SE_INSTANCE_STATS[] stats_list, Int32 stats_count );


		#endregion

	}
}
