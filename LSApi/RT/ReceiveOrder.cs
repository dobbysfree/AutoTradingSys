using System.Text;
using XA_DATASETLib;

namespace LSApi.RT
{
    internal class ReceiveOrder
    {
        XARealClass xa;
        public ReceiveOrder()
        {
            xa = new XARealClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "SC0.res";
            xa.ReceiveRealData += ReceiveMessage;
            xa.AdviseRealData();
        }

        public void ReceiveMessage(string tr)
        {
            try
            {
                //if (xa.GetFieldData("OutBlock", "accno") != Conf.IConfig["Ebest:Acnt"]) return;

                string code = xa.GetFieldData("OutBlock", "shtcode").Substring(1);         // Stock Ticker
                App.TradingItems.TryGetValue(code, out Item im);
                if (im == null) return;

                // 주문체결구분(01:Order, 02:Replace, 03:Cancel, 11:Execution, 12:Replace Confirmation, 13:Cancel Confirmation, 14:Rejected)
                string gubun    = xa.GetFieldData("OutBlock", "ordchegb");
                string ordno    = xa.GetFieldData("OutBlock", "ordno");                    // Order NO                 
                long ord_qty    = long.Parse(xa.GetFieldData("OutBlock", "ordqty"));       // Order Quantity
                string bs       = xa.GetFieldData("OutBlock", "bnstp");                    // 1:ASK, 2:BID
                string orgordno = xa.GetFieldData("OutBlock", "orgordno");                 // Original Order NO
                string name     = xa.GetFieldData("OutBlock", "hname");                    // Stock Name
                long ord_prc    = long.Parse(xa.GetFieldData("OutBlock", "ordprice"));     // Order Price

                StringBuilder sb = new StringBuilder();
                sb.Append("type:").Append(Dic.Chegyul[gubun]).Append("Acceptance").Append(", ");
                sb.Append("code:").Append(code).Append(", ");
                sb.Append("name:").Append(name).Append(", ");
                sb.Append("qty:").Append(ord_qty).Append(", ");
                sb.Append("prc:").Append(ord_prc).Append(", ");
                sb.Append("side:").Append(Dic.BuySell[bs]).Append(", ");
                sb.Append("ordno:").Append(ordno).Append(", ");
                sb.Append("orgordno:").Append(orgordno).Append(", ");
                sb.Append("askprc:").Append(im.HogaAsk.price).Append(", ");
                sb.Append("askrem:").Append(im.HogaAsk.rem).Append(", ");
                sb.Append("bidprc:").Append(im.HogaBid.price).Append(", ");
                sb.Append("bidrem:").Append(im.HogaBid.rem).Append(", ");
                sb.Append("pos:").Append(im.CrntPos);
                Conf.ILog.Information(sb.ToString());
            }
            catch (Exception ex)
            {
                Program.ILog.Error(ex.ToString());
            }
        }
    }
}