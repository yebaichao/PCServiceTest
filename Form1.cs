using PCDLRN;
using PCService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PCServiceTest
{
    public partial class FormPCService : Form
    {
        public FormPCService()
        {
            InitializeComponent();
        }
        private PCDLRN.Application pcApp { get; set; }
        /// <summary>
        /// 竖直探针数组//测针名，X，Y，Z，直径，I，J，K，测头名称
        /// </summary>
        private string[] prbsVertical = null;//竖直探针数组//测针名，X，Y，Z，直径，I，J，K
        /// <summary>
        /// 横向探针数组//测针名，X，Y，Z，直径，I，J，K，测头名称
        /// </summary>
        private string[] prbsMulti = null;//横向探针数组//测针名，X，Y，Z，直径，I，J，K
        /// <summary>
        /// 获取PC-DMIS程序对象
        /// </summary>
        /// <returns></returns>
        private PCDLRN.Application GetPcdmisApp(bool showPC)
        {
            try
            {
                var comType = Type.GetTypeFromProgID("PCDLRN.Application");
                var comObj = Activator.CreateInstance(comType);

                var pcdApp = comObj as PCDLRN.Application;
                if (pcdApp != null)
                {
                    pcdApp.Visible = showPC;
                }
                pcApp = pcdApp;
                return pcdApp;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取测头竖直和横向测针直径大小
        /// </summary>
        /// <param name="ProbName">测头1的名称</param>
        /// <param name="ProbName2">测头2的名称</param>
        /// <param name="vertical">垂直探针</param>
        /// <param name="multi">横向</param>
        /// <param name="pcdmisApp">如果传入为null则显示pcdmis，如果不为null则按照传入的pc对象显示pcdmis</param>
        /// <param name="ProbeArm">如果是关节臂则为空字符串</param>
        public void GetProbeTips(string ProbName, out string[] vertical, out string[] multi, PCDLRN.Application PcdmisApp)
        {
            vertical = null;
            multi = null;
            try
            {                
                if (PcdmisApp.ActivePartProgram == null)
                {
                    return;
                }
                Tips tips = PcdmisApp.ActivePartProgram.Probes.Item(ProbName).Tips;
                string probeFullName = PcdmisApp.ActivePartProgram.Probes.Item(ProbName).FullName;
                List<string> tempList1 = new List<string>();
                List<string> tempList2 = new List<string>();
                #region 获取探针列表
                for (int i = 1; i <= tips.Count; i++)
                {

                    PointData xyz = tips.Item(i).XYZ;
                    PointData ijk = tips.Item(i).IJK;
                    double diam = tips.Item(i).diam;
                    string tipName = tips.Item(i).ID;
                    if (ijk.I == 0 && ijk.J == 0 && ijk.K == 1)
                    {
                        //获取竖直测针
                        tempList1.Add(tipName + "," + (-xyz.X).ToString() + "," + (-xyz.Y).ToString() + "," + (-xyz.Z).ToString() + "," + diam.ToString() + "," + (-ijk.I).ToString() + "," + (-ijk.J).ToString() + "," + (-ijk.K).ToString() + "," + ProbName);

                    }
                    else
                    {
                        //获取横向测针
                        tempList2.Add(tipName + "," + (-xyz.X).ToString() + "," + (-xyz.Y).ToString() + "," + (-xyz.Z).ToString() + "," + diam.ToString() + "," + (-ijk.I).ToString() + "," + (-ijk.J).ToString() + "," + (-ijk.K).ToString() + "," + ProbName);
                    }
                }
                PcdmisApp.ActivePartProgram.Probes.Visible = false;          //关闭测头工具对话框
                #endregion

                vertical = tempList1.ToArray();
                multi = tempList2.ToArray();
            }
            catch (Exception ex) { ExceptionLogHelp.WriteLog(ex); }
        }
        /// <summary>
        /// 添加圆特征
        /// </summary>
        /// <param name="DmisCommands"></param>
        /// <param name="cqnpts"></param>
        /// <param name="PRBs"></param>
        /// <param name="Diameter"></param>
        /// <param name="InOrOut">1为外圆，2为内圆</param>
        private void AddCircle(Commands DmisCommands, List<Cq_npt> cq_Npts, List<Cq_prb> PRBs, string Diameter, int InOrOut)
        {
            double prbIndex = 0;//记录测针编号
            Command DmisCommand = DmisCommands.Add(OBTYPE.MEASURED_CIRCLE, true);
            DmisCommand.Marked = true;
            //' Set 理论 X  = 0
            bool retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.THEO_X, 0);
            //' Set 理论 Y  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.THEO_Y, 0);
            //' Set 理论 Z  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.THEO_Z, 0);
            //' Set 理论值 I  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.THEO_I, 0);
            //' Set 理论值J  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.THEO_J, 0);
            //' Set 理论值K  = 1
            retval = DmisCommand.PutText("1", ENUM_FIELD_TYPES.THEO_K, 0);
            //' Set 理论直径  = 10
            retval = DmisCommand.PutText(Diameter, ENUM_FIELD_TYPES.THEO_DIAM, 0);
            //' Set 测定值X  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.MEAS_X, 0);
            //' Set 测定值Y  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.MEAS_Y, 0);
            //' Set 测定值Z  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.MEAS_Z, 0);
            //' Set 测定值I  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.MEAS_I, 0);
            //' Set 测定值J  = 0
            retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.MEAS_J, 0);
            //' Set 测定值K  = 1
            retval = DmisCommand.PutText("1", ENUM_FIELD_TYPES.MEAS_K, 0);
            //' Set 测定直径  = 10
            retval = DmisCommand.PutText(Diameter, ENUM_FIELD_TYPES.MEAS_DIAM, 0);
            //' Set 标识  = 圆2
            retval = DmisCommand.PutText("TIPCIRCLE", ENUM_FIELD_TYPES.ID, 0);
            //' Set 坐标类型  = 直角坐标
            retval = DmisCommand.SetToggleString(1, ENUM_FIELD_TYPES.COORD_TYPE, 0);
            //' Set 内/外  = 外
            retval = DmisCommand.SetToggleString(InOrOut, ENUM_FIELD_TYPES.INOUT_TYPE, 0);
            //' Set 最佳拟合数学类型  = 最小二乘方
            retval = DmisCommand.SetToggleString(1, ENUM_FIELD_TYPES.BF_MATH_TYPE, 0);
            //' Set 2D/3D  = Z 正
            retval = DmisCommand.SetToggleString(4, ENUM_FIELD_TYPES.MEASURED_2D3D_TYPE, 0);
            //' Set 触测点数  = 4
            retval = DmisCommand.PutText("4", ENUM_FIELD_TYPES.N_HITS, 0);

            DmisCommand = DmisCommands.Add(OBTYPE.MOVE_CLEARP, true);
            DmisCommand.Marked = true;

            foreach (Cq_npt cq_Npt in cq_Npts)
            {
                if (cq_Npt.typ == "PRB")
                {
                    if (prbIndex != cq_Npt.prbidx)
                    {
                        prbIndex = cq_Npt.prbidx;
                        Cq_prb cq_Prb = PRBs.Where(x => x.prbidx == cq_Npt.prbidx).FirstOrDefault();

                        DmisCommand = DmisCommands.Add(OBTYPE.SET_ACTIVE_TIP, true);
                        DmisCommand.Marked = true;
                        //' Set 标识  = TIP4
                        retval = DmisCommand.PutText(cq_Prb.prbnam, ENUM_FIELD_TYPES.REF_ID, 0);
                        ////' Set 测尖 I  = 1
                        //retval = DmisCommand.PutText("1", ENUM_FIELD_TYPES.TIP_I, 0);
                        ////' Set 测尖 J  = 0
                        //retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.TIP_J, 0);
                        ////' Set 测尖 K  = 0
                        //retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.TIP_K, 0);
                        ////' Set 理论角度  = 0
                        //retval = DmisCommand.PutText("0", ENUM_FIELD_TYPES.THEO_ANGLE, 0);
                    }
                }
                else 
                {
                    DmisCommand = DmisCommands.Add(OBTYPE.BASIC_HIT, true);
                    DmisCommand.Marked = true;
                    //' Set 理论 X  = 5
                    retval = DmisCommand.PutText(cq_Npt.reafld[0].ToString(), ENUM_FIELD_TYPES.THEO_X, 0);
                    //' Set 理论 Y  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[1].ToString(), ENUM_FIELD_TYPES.THEO_Y, 0);
                    //' Set 理论 Z  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[2].ToString(), ENUM_FIELD_TYPES.THEO_Z, 0);
                    //' Set 理论值 I  = 1
                    retval = DmisCommand.PutText(cq_Npt.reafld[3].ToString(), ENUM_FIELD_TYPES.THEO_I, 0);
                    //' Set 理论值J  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[4].ToString(), ENUM_FIELD_TYPES.THEO_J, 0);
                    //' Set 理论值K  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[5].ToString(), ENUM_FIELD_TYPES.THEO_K, 0);
                    //' Set 测定值X  = 5
                    retval = DmisCommand.PutText(cq_Npt.reafld[0].ToString(), ENUM_FIELD_TYPES.MEAS_X, 0);
                    //' Set 测定值Y  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[1].ToString(), ENUM_FIELD_TYPES.MEAS_Y, 0);
                    //' Set 测定值Z  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[2].ToString(), ENUM_FIELD_TYPES.MEAS_Z, 0);
                    //' Set 测定值I  = 1
                    retval = DmisCommand.PutText(cq_Npt.reafld[3].ToString(), ENUM_FIELD_TYPES.MEAS_I, 0);
                    //' Set 测定值J  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[4].ToString(), ENUM_FIELD_TYPES.MEAS_J, 0);
                    //' Set 测定值K  = 0
                    retval = DmisCommand.PutText(cq_Npt.reafld[5].ToString(), ENUM_FIELD_TYPES.MEAS_K, 0);
                    //' Set 学习模式  = 常规
                    retval = DmisCommand.SetToggleString(1, ENUM_FIELD_TYPES.NORM_RELEARN, 0);
                    //' Set 使用理论矢量  = 是
                    retval = DmisCommand.SetToggleString(2, ENUM_FIELD_TYPES.USE_THEO, 0);
                }
            }
            DmisCommand = DmisCommands.Add(OBTYPE.MOVE_CLEARP, true);
            DmisCommand.Marked = true;

            DmisCommand = DmisCommands.Add(OBTYPE.END_MEASURED_FEATURE, true);
            DmisCommand.Marked = true;
        }
        private void button_Test_Click(object sender, EventArgs e)
        {
            PCServiceRepository PCService = new PCServiceRepository();//获取PC服务
            List<Cq_prb> PRBs = new List<Cq_prb>();
            Cq_prb cq_Prb = new Cq_prb();
            PCDLRN.Application PcdmisApp = GetPcdmisApp(true);

            GetProbeTips("ProbeName", out prbsVertical, out prbsMulti, PcdmisApp);//获取竖向测针和横向测针

            //此处根据需要，使用竖向测针就用prbsVertical，如果用横向测针就用prbsMulti
            Cq_prb.SetSize(ref PRBs, prbsVertical.Length);
            cq_Prb.CovertGearPth(ref PRBs, prbsVertical);

            Cq_csy cq_Csy = new Cq_csy();
            cq_Csy.ConertPcDmisAln(cq_Csy, "ALN");//获取当前坐标系矩阵

            //测量点初始化，存放测量点的坐标信息和矢量信息
            CpointD cpointD = new CpointD() {
                x=0,
                y=0,
                z=0,
                u=0,
                v=0,
                w=1
            };
            List<CpointD> cpointDs = new List<CpointD>() { cpointD };//特征测量点列表
            List<Cq_npt> cq_Npts = new List<Cq_npt>();//最后获取的测量点和路径

            int aoi = 1;//外圆为1，内圆为-1
            double safeDis = 20;//外圆：安全直径=圆直径+安全距离*2，如果是内圆：安全直径=圆直径-安全距离*2
            PCService.gear_pth(ref cpointDs, ref PRBs, cq_Csy, ref cq_Npts, aoi, safeDis, 0, -1, "N", "N", 5.0, 0.0, 0.0, 1.0, "N", "N");
            int InOrOut = 1;//外圆为1，内圆为2;
            string Diameter = "20";
            AddCircle(PcdmisApp.ActivePartProgram.Commands, cq_Npts, PRBs, Diameter, InOrOut);
        }
    }
}
