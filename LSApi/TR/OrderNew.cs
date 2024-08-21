using System.Text;
using XA_DATASETLib;

namespace LSApi.TR
{
    internal class OrderNew
    {
        static object lockey { get; set; } = new object();

        #region Main
        public OrderNew()
        {
            xa = new XAQueryClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "CSPAT00600.res";
            xa.ReceiveMessage += ReceiveMessage;
        }
        #endregion

        #region [CSPAT00600] Request
        static XAQueryClass xa;
        public static void Request(string code, string name, BuySell bs, long prc, long qty, string ptnCd)
        {
            try
            {
                lock (lockey)
                {
                    string blockNm = "CSPAT00600InBlock1";
                    xa.SetFieldData(blockNm, "AcntNo", 0, App.IConf["Ebest:Acnt"]);          // Account NO
                    xa.SetFieldData(blockNm, "InptPwd", 0, App.IConf["Ebest:AcntPW"]);       // Account PW
                    xa.SetFieldData(blockNm, "IsuNo", 0, code);                              // Stock Ticker
                    xa.SetFieldData(blockNm, "OrdQty", 0, qty.ToString());                   // Order Quantity
                    xa.SetFieldData(blockNm, "OrdPrc", 0, prc.ToString());                   // Order Price
                    xa.SetFieldData(blockNm, "BnsTpCode", 0, bs == BuySell.SELL ? "1" : "2");// 1:ASK, 2:BID
                    xa.SetFieldData(blockNm, "OrdprcPtnCode", 0, ptnCd);                     // 00:LIMIT, 03:MARKET, 61:Pre-market Closing Price, 82:After-hours Single Price
                    xa.SetFieldData(blockNm, "MgntrnCode", 0, "000");                        // Margin Transaction Code
                    xa.SetFieldData(blockNm, "LoanDt", 0, "");                               // Loan Date                                
                    xa.SetFieldData(blockNm, "OrdCndiTpCode", 0, "0");                       // 0:None, 1:IOC(mmediate or Cancel), 2:FOK(Fill or Kill)
                    var reqID = xa.Request(false);

                    if (reqID < 0) Program.ILog.Warning($"{bs} Order Request Failure > {code}, {reqID}");
                    else Program.ILog.Information($"Type:{bs} Request, Code:{code}, Name:{name}, Side:{bs}, Qty:{qty}, Prc:{prc}, PtnCD:{ptnCd}");
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
            if (MsgCode == "00039" || MsgCode == "00040") return;

            Program.ILog.Warning($"Order Failure Received > ErrCD:{MsgCode}, ErrMsg:{Msg}");
        }
        #endregion
    }
}