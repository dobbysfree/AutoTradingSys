using System.Text;
using XA_DATASETLib;

namespace LSApi.TR
{
    internal class OrderReplace
    {
        static object lockey { get; set; } = new object();

        #region Main
        public OrderReplace()
        {
            xa = new XAQueryClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "CSPAT00700.res";
            xa.ReceiveMessage += ReceiveMessage;
        }
        #endregion

        #region [CSPAT00700] Request
        static XAQueryClass xa;
              public static void Request(Item im, string ordno, long price, long qty)
        {
            try
            {
                lock (lockey)
                {
                    string blockNm = "CSPAT00700InBlock1";
                    xa.SetFieldData(blockNm, "OrgOrdNo", 0, ordno);                     // Order NO
                    xa.SetFieldData(blockNm, "AcntNo", 0, App.IConf["Ebest:Acnt"]);     // Account NO
                    xa.SetFieldData(blockNm, "InptPwd", 0, App.IConf["Ebest:AcntPW"]);  // Account PW
                    xa.SetFieldData(blockNm, "IsuNo", 0, im.Code);                      // Stock Ticker
                    xa.SetFieldData(blockNm, "OrdQty", 0, qty.ToString());              // Order Quantity
                    xa.SetFieldData(blockNm, "OrdprcPtnCode", 0, "00");                 // Order Type Code
                    xa.SetFieldData(blockNm, "OrdCndiTpCode", 0, "0");                  // Order Condition Type
                    xa.SetFieldData(blockNm, "OrdPrc", 0, price.ToString());            // Order Price
                    var reqID = xa.Request(false);

                    if (reqID < 0) Program.ILog.Warning($"Replace Request Failure > Code:{im.Code}, ReqID:{reqID}");
                    else Program.ILog.Information($"Type:Replace Request, Code:{im.Code}, Name:{im.Name}, OrdNO:{ordno}, Qty:{qty}, Prc:{price}, ReqID:{reqID}");
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
            if (MsgCode == "00132") return;

            Program.ILog.Warning($"Replace Failure Received > ErrCD:{MsgCode}, ErrMsg:{Msg}");
        }
        #endregion
    }
}