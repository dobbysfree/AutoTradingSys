using System.Text;
using XA_DATASETLib;

namespace LSApi.TR
{
    internal class OrderCancel
    {
        static object lockey { get; set; } = new object();

        #region Main
        public OrderCancel()
        {
            xa = new XAQueryClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "CSPAT00800.res";
            xa.ReceiveMessage += ReceiveMessage;
        }
        #endregion

        #region [CSPAT00800] Request
        static XAQueryClass xa;
        public static void Request(Item im, string ordno, long qty)
        {
            try
            {
                lock (lockey)
                {
                    string blockNm = "CSPAT00800InBlock1";
                    xa.SetFieldData(blockNm, "OrgOrdNo", 0, ordno);                     // Order NO
                    xa.SetFieldData(blockNm, "AcntNo", 0, App.IConf["Ebest:Acnt"]);     // Account NO
                    xa.SetFieldData(blockNm, "InptPwd", 0, App.IConf["Ebest:AcntPW"]);  // Account PW
                    xa.SetFieldData(blockNm, "IsuNo", 0, im.Code);                      // Stock Ticker
                    xa.SetFieldData(blockNm, "OrdQty", 0, qty.ToString());              // Order Quantity
                    var reqID = xa.Request(false);

                    if (reqID < 0) Program.ILog.Warning("Cancel Request Failure > " + im.Code + ", " + reqID);
                    else Program.ILog.Information($"Type:Cancel Request, Code:{im.Code}, Name:{im.Name}, OrdNO:{ordno}, Qty:{qty}, ReqID:{reqID}");
                }
            }
            catch (Exception ex)
            {
                Program.ILog.Error(ex.ToString());
            }
        }
        #endregion

        #region Receive Message
        private void ReceiveMessage(bool IsError, string MsgCode, string Msg)
        {
            // Order Processed Successfully
            if (MsgCode == "00156") return;

            Program.ILog.Warning($"Cancel Failure Received > ErrCD:{MsgCode}, ErrMsg:{Msg}");
        }
        #endregion
    }
}