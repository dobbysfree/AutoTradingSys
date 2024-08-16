using System.Text;
using XA_DATASETLib;

namespace LSApi.TR
{
    internal class Account
    {
        #region [CSPAQ12200] Request and receive account balance inquiry
        static XAQueryClass cspaq12200;
        public static void ReqDeposit()
        {
            cspaq12200 = new XAQueryClass();
            cspaq12200.ResFileName = App.IConf["LS:Res"] + "CSPAQ12200.res";
            cspaq12200.ReceiveData += RcvCSPAQ12200;

            cspaq12200.SetFieldData("CSPAQ12200InBlock1", "RecCnt", 0, "1");
            cspaq12200.SetFieldData("CSPAQ12200InBlock1", "MgmtBrnNo", 0, "");
            cspaq12200.SetFieldData("CSPAQ12200InBlock1", "AcntNo", 0, App.IConf["LS:Acnt"]);
            cspaq12200.SetFieldData("CSPAQ12200InBlock1", "Pwd", 0, App.IConf["LS:AcntPW"]);
            cspaq12200.SetFieldData("CSPAQ12200InBlock1", "BalCreTp", 0, "0");

            var result = cspaq12200.Request(false);
            if (result < 0) Program.ILog.Warning("[CSPAQ12200] Failed balance inquiry");
        }

        static void RcvCSPAQ12200(string trCode)
        {
            try
            {
                string trout = trCode + "OutBlock2";

                var deposit         = long.Parse(cspaq12200.GetFieldData(trout, "Dps", 0));            // Deposit
                App.Balance         = long.Parse(cspaq12200.GetFieldData(trout, "MnyOrdAbleAmt", 0));  // Order available amount
                long DpsastTotamt   = long.Parse(cspaq12200.GetFieldData(trout, "DpsastTotamt", 0));   // Deposit toal amount

                Program.ILog.Information($"Finish balance inquiry > Dps='{deposit.ToString("N0")}', MnyOrdAbleAmt='{App.Balance.ToString("N0")}', DpsastTotamt='{DpsastTotamt.ToString("N0")}'");
            }
            catch (Exception ex)
            {
                Program.ILog.Error(ex.ToString());
            }
            finally
            {
                Program.Waiting?.TrySetResult(true);
            }
        }
        #endregion


        #region [t0424] Inquire stock holdings during market hours or after-hours single price session
        static XAQueryClass t0424;
        public static void ReqBalanceStocks()
        {
            t0424 = new XAQueryClass();
            t0424.ResFileName = App.IConf["LS:Res"] + "t0424.res";
            t0424.ReceiveData += RcvT0424;

            t0424.SetFieldData("t0424InBlock", "accno", 0, App.IConf["LS:Acnt"]);
            t0424.SetFieldData("t0424InBlock", "passwd", 0, App.IConf["LS:AcntPW"]);
            t0424.SetFieldData("t0424InBlock", "prcgb", 0, "1");        // 1:Average price
            t0424.SetFieldData("t0424InBlock", "chegb", 0, "2");        // 2:Based on execution
            t0424.SetFieldData("t0424InBlock", "dangb", 0, "0");        // 0:Regular trading hours, 1:After-hours single price
            t0424.SetFieldData("t0424InBlock", "charge", 0, "1");       // 1:Including fees
            t0424.SetFieldData("t0424InBlock", "cts_expcode", 0, "");
            var result = t0424.Request(false);

            if (result < 0) Program.ILog.Warning("[t0424] Failed stock hodling inquiry");
        }

        static void RcvT0424(string trCode)
        {
            try
            {
                string trout = trCode + "OutBlock1";

                for (int i = 0; i < t0424.GetBlockCount(trout); i++)
                {
                    var im          = new Item();
                    im.Code         = t0424.GetFieldData(trout, "expcode", i);
                    im.Name         = t0424.GetFieldData(trout, "hname", i);
                    im.TotQty       = int.Parse(t0424.GetFieldData(trout, "janqty", i));        // Quantity in balance
                    im.PosQty       = int.Parse(t0424.GetFieldData(trout, "mdposqt", i));       // Available quantity for ASK
                    im.AvgPrc       = long.Parse(t0424.GetFieldData(trout, "pamt", i));         // Average price
                    im.PosPer       = decimal.Parse(t0424.GetFieldData(trout, "sunikrt", i));   // Profit rate
                    im.Price        = long.Parse(t0424.GetFieldData(trout, "price", i));        // Current price
                    long appamt     = long.Parse(t0424.GetFieldData(trout, "appamt", i));       // Evaluated amount
                    long dtsunik    = long.Parse(t0424.GetFieldData(trout, "dtsunik", i));      // Evaluated profit/loss 

                    Program.ILog.Information($"StockHolding > Code='{im.Code}', Name='{im.Name}', QtyBalance='{im.TotQty}', AbleQtyAsk='{im.PosQty}', " +
                        $"AvgPrc='{im.AvgPrc}', CrntPrc='{im.Price}', ProfitRate='{im.PosPer}', EvalAmt='{appamt}', EvalProfitLoss='{dtsunik}'");

                    App.TradingItems[im.Code] = im;
                }
            }
            catch (Exception ex)
            {
                Program.ILog.Error(ex.ToString());
            }
            finally
            {
                Program.Waiting?.TrySetResult(true);
            }
        }
        #endregion
    }
}