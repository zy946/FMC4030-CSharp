using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

///
/// FMC4030-Dll.dll 32位
/// /
namespace FMC4030Contorller
{
    /// <summary>
    /// FMC4030控制器 
    /// </summary>
    class FMC4030
    {
        private const string dllName = "./dll/FMC4030-Dll.dll";
        /// <summary>
        /// 查看返回值信息
        /// </summary>
        /// <param name="errorCode">功能方法返回错误码</param>
        /// <returns>返回错误码对应信息</returns>
        public static string CheckError(int errorCode)
        {
            return errorCode switch
            {
                0 => "执行成功",
                -1 => $"{errorCode},连接失败",
                -4 => $"{errorCode},数据建立失败",
                -5 => $"{errorCode},数据发送失败,检查网线连接、检查IP地址及端口号、重启控制器",
                -6 => $"{errorCode},数据接收控制,检查网线连接、检查IP地址及端口号、重启控制器",
                -7 => $"{errorCode},接收数据错误,检查网络连接",
                -8 => $"{errorCode},空指针错误,检查传入参数是否为空指针",
                _ => $"{errorCode},未知错误",
            };
        }

        /// <summary>
        /// 打开（连接）设备
        /// </summary>
        /// <param name="id">分配给控制器的id号，此id号唯一不能重复</param>
        /// <param name="ip">待连接的控制器的IP地址，出厂默认控制器IP为“192.168.0.30”</param>
        /// <param name="port">待连接的控制器端口号，出厂默认8088</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Open_Device", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int OpenDevice(int id, [MarshalAs(UnmanagedType.LPStr)] string ip, int port);

        /// <summary>
        /// 关闭（断开）设备
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Close_Device", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int CloseDevice(int id);

        /// <summary>
        /// 单轴移动
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <param name="axis">需要运行的轴号 0、1、2 分别对应 X、Y、Z 轴</param>
        /// <param name="pos">运行的距离，区别正负，单位 mm</param>
        /// <param name="speed">运行的速度，只能为正数，单位 mm/s</param>
        /// <param name="acc">运行的加速度，只能为正数，单位 mm/s²</param>
        /// <param name="dec">运行的减速度，只能为正数，单位 mm/s²</param>
        /// <param name="mode">运行模式。1：相对运动，2：绝对运动</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Jog_Single_Axis", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int JogSingleAxis(int id, int axis, float pos, float speed, float acc, float dec, int mode);

        /// <summary>
        /// 单轴停止
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <param name="axis">待停止的轴号 0、1、2 分别对应 X、Y、Z 轴</param>
        /// <param name="mode">1：减速停止，2：立即停止</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Stop_Single_Axis", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int StopSingleAxis(int id, int axis, int mode);

        /// <summary>
        /// 检查某轴是否为停止状态，用于判断某轴的运行状态 
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <param name="axis">0、1、2 分别对应 X、Y、Z 轴</param>
        /// <returns>1：标志轴为停止状态，0 标志轴为运行状态</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Check_Axis_Is_Stop", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int CheckAxisIsStop(int id, int axis);

        /// <summary>
        /// 获取某轴当前实际位置，此位置为控制卡内部计数产生，若电机发生堵转或卡滞，则此位置不准确
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <param name="axis">0、1、2 分别对应 X、Y、Z 轴</param>
        /// <param name="pos">传入浮点数指针或四字节数组指针，调用此函数后数据将会更新到变量中，单位 mm</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Get_Axis_Current_Pos", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int GetAxisCurrentPos(int id, int axis, ref float pos);

        /// <summary>
        /// 获取某轴当前运行速度
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <param name="axis">0、1、2 分别对应 X、Y、Z 轴</param>
        /// <param name="speed">传入浮点数指针或四字节数组指针，调用此函数后数据将会更新到变量中，单位 mm/s</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Get_Axis_Current_Speed", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int GetAxisCurrentSpeed(int id, int axis, ref float speed);

        /// <summary>
        /// 控制某轴执行回零
        /// </summary>
        /// <param name="id">分配给控制器的id号</param>
        /// <param name="axis">0、1、2 分别对应 X、Y、Z 轴</param>
        /// <param name="homeSpeed">回零速度，正数，单位 mm/s</param>
        /// <param name="homeAccDec">回零加减速度，正数，单位 mm/s²</param>
        /// <param name="homeFallStep">回零脱落距离，正数，单位 mm。此参数为回零完成后远离限位开关的距离。</param>
        /// <param name="homeDir">回零方向，1：正限位回零，2：负限位回零</param>
        /// <returns>见CheckError</returns>
        [DllImport(dllName, EntryPoint = "FMC4030_Home_Single_Axis", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int HomeSingleAxis(int id, int axis, float homeSpeed, float homeAccDec, float homeFallStep, int homeDir);
    }



}
