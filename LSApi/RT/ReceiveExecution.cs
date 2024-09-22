using System.Text;
using XA_DATASETLib;

namespace LSApi.RT
{
    internal class ReceiveExecution
    {
        XARealClass xa;
        public ReceiveExecution()
        {
            xa = new XARealClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "SC1.res";
            xa.ReceiveRealData += ReceiveMessage;
            xa.AdviseRealData();
        }

          public void ReceiveMessage(string tr)
        {
            try
            {
                //if (xa.GetFieldData("OutBlock", "accno") != Conf.IConfig["Ebest:Acnt"]) return;

                string code = xa.GetFieldData("OutBlock", "shtnIsuno").Substring(1);   // Stock Ticker
                App.TradingItems.TryGetValue(code, out Item im);
                if (im == null) return;

                long exec_qty   = long.Parse(xa.GetFieldData("OutBlock", "execqty"));      // Fulfilled Quantity
                string bs       = xa.GetFieldData("OutBlock", "bnstp");                    // 1:ASK, 2:BID
                string name     = xa.GetFieldData("OutBlock", "Isunm");                    // Stock Name
                long unercqty   = long.Parse(xa.GetFieldData("OutBlock", "unercqty"));     // Unfulfilled Quantity
                long deposit    = long.Parse(xa.GetFieldData("OutBlock", "deposit"));      // Deposit
                long ordablemny = long.Parse(xa.GetFieldData("OutBlock", "ordablemny"));   // Available Cash for Orders
                string ordno    = xa.GetFieldData("OutBlock", "ordno");                    // Order NO
                string exectime = xa.GetFieldData("OutBlock", "exectime");                 // Execution Time
                long exec_prc   = long.Parse(xa.GetFieldData("OutBlock", "execprc"));      // Execution Price


                //StringBuilder sb = new StringBuilder();
                //sb.Append("type:Executed").Append(", ");
                //sb.Append("code:").Append(code).Append(", ");
                //sb.Append("name:").Append(name).Append(", ");
                //sb.Append("execqty:").Append(exec_qty).Append(", ");
                //sb.Append("execprc:").Append(exec_prc).Append(", ");
                //sb.Append("side:").Append(Dic.BuySell[bs]).Append(", ");
                //sb.Append("unercqty:").Append(unercqty).Append(", ");
                //sb.Append("exectime:").Append(exectime).Append(", ");
                //sb.Append("ordno:").Append(ordno).Append(", ");
                //sb.Append("posqty:").Append(im.PosQty).Append(", ");
                //sb.Append("askprc:").Append(im.HogaAsk.price).Append(", ");
                //sb.Append("askrem:").Append(im.HogaAsk.rem).Append(", ");
                //sb.Append("bidprc:").Append(im.HogaBid.price).Append(", ");
                //sb.Append("bidrem:").Append(im.HogaBid.rem).Append(", ");
                //sb.Append("pos:").Append(im.CrntPos);
                //Conf.ILog.Information(sb.ToString());
            }
            catch (Exception ex)
            {
                Program.ILog.Error(ex.ToString());
            }
        }
    }
}