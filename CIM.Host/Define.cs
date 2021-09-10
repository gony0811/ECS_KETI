using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.Host
{


    public class EQPDefine
    {
        public static string IQC = "IQ01";
        public static string POL = "PL01";
        public static string TFOP = "TP01";
        public static string TCRD = "TY01";
        public static string ATT = "A01";
        public static string DCOF = "DP01";
        public static string DFOF = "DF01";
        public static string DCRD = "DY01";
        public static string THERMAL = "HP01";
        public static string BPL = "BL01";
        public static string TLAMI = "TL01";
        public static string DLAMI = "DL01";
        public static string BLAMI = "LB01";
        public static string BEND = "B01";
        public static string AUTOCLAVE = "AC01";
        public static string BLASER = "BC01";
        public static string PLASER = "PC01";
        public static string LOTMARKING = "LK01";
    }

    public class gDefine // #Define
    {
        public static int DEF_EQPID_SIZE = 40;    //ex)LAMI
        public static int DEF_MATERIAL_AMOUNT = 1;
        public static int DEF_MATERIAL_PORT = 2;
        public static int DEF_TOTAL_ALARM = 700;

        public static int DEF_DATAID_SIZE = 4;
        public static int DEF_CEID_SIZE = 3;
        public static int DEF_RPTID_SIZE = 3;
        public static int DEF_REASONCODE_SIZE = 20;
        public static int DEF_DESCRIPTION_SIZE = 40;
        public static int DEF_UNITID_SIZE = 40;
        public static int DEF_ALID_SIZE = 10;
        public static int DEF_ALTX_SIZE = 120;
        public static int DEF_MATERIALID_SIZE = 40;
        public static int DEF_MATERIALTYPE_SIZE = 20;
        public static int DEF_MATERIALUSAGE_SIZE = 20;
        public static int DEF_PPID_SIZE = 40;
        public static int DEF_OLDPPID_SIZE = 40;
        public static int DEF_PARAMETERNAME_SIZE = 20;
        public static int DEF_PARAMETERVALUE_SIZE = 40;
        public static int DEF_CELLID_SIZE = 40;
        public static int DEF_PRODUCTID_SIZE = 40;
        public static int DEF_STEPID_SIZE = 40;
        public static int DEF_EQ_MAT_BAT_ID_SIZE = 40;
        public static int DEF_EQ_MAT_BAT_NAME_SIZE = 40;
        public static int DEF_EQ_MAT_ID_SIZE = 40;
        public static int DEF_EQ_MAT_TYPE_SIZE = 40;
        public static int DEF_EQ_MAT_TOTAL_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_USE_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_ASSEM_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_NG_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_MAINQTY_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_PRODUCT_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_PROCE_USE_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_PROCE_ASSEM_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_PROC_NG_QTY_SIZE = 10;
        public static int DEF_EQ_MAT_SUPPLY_QUEST_QTY_SIZE = 10;
        public static int DEF_PARENT_LOT_SIZE = 40;
        public static int DEF_RFID_SIZE = 40;
        public static int DEF_PLAN_QTY_SIZE = 10;
        public static int DEF_PROCESSED_QTY_SIZE = 10;
        public static int DEF_PROCESS_JOB_SIZE = 40;
        public static int DEF_READER_ID_SIZE = 10;
        public static int DEF_BATCHLOT_SIZE = 40;

        public static int DEF_LABEL_OPTIONCODE_SIZE = 10;
        public static int DEF_LABEL_CELLID_SIZE = 40;
        public static int DEF_LABEL_PRODUCTID_SIZE = 40;
        public static int DEF_LABEL_OPTIONINFO_SIZE = 80;
        public static int DEF_LABEL_LABELID_SIZE = 80;
    }
}
