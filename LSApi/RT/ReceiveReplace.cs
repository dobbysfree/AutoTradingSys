using System.Text;
using XA_DATASETLib;

namespace LSApi.RT
{
    internal class ReceiveReplace
    {
        XARealClass xa;
        public ReceiveReplace()
        {
            xa = new XARealClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "SC2.res";
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

                long mdfy_qty   = long.Parse(xa.GetFieldData("OutBlock", "mdfycnfqty"));   // Replace Confirmed Quantity
                string name     = xa.GetFieldData("OutBlock", "Isunm");                    // Stock Name
                string bs       = xa.GetFieldData("OutBlock", "bnstp");                    // 1:ASK, 2:BID
                string ordno    = xa.GetFieldData("OutBlock", "ordno");                    // Order NO
                string orgordno = xa.GetFieldData("OutBlock", "orgordno");                 // Original Order NO
                long ord_qty    = long.Parse(xa.GetFieldData("OutBlock", "ordqty"));       // Order Quantity                
                long ord_prc    = long.Parse(xa.GetFieldData("OutBlock", "ordprc"));       // Order Price
                long mdfy_prc   = long.Parse(xa.GetFieldData("OutBlock", "mdfycnfprc"));   // Replace Confirmed Price
                long unercqty   = long.Parse(xa.GetFieldData("OutBlock", "unercqty"));     // Unfulfilled Quantity

                //StringBuilder sb = new StringBuilder();
                //sb.Append("type:Replace").Append(", ");
                //sb.Append("code:").Append(code).Append(", ");
                //sb.Append("name:").Append(name).Append(", ");
                //sb.Append("side:").Append(Dic.BuySell[bs]).Append(", ");
                //sb.Append("ordno:").Append(ordno).Append(", ");
                //sb.Append("orgordno:").Append(orgordno).Append(", ");
                //sb.Append("ordqty:").Append(ord_qty).Append(", ");
                //sb.Append("mdfyqty:").Append(mdfy_qty).Append(", ");
                //sb.Append("ordprc:").Append(ord_prc).Append(", ");
                //sb.Append("mdfyprc:").Append(mdfy_prc).Append(", ");
                //sb.Append("unercqty:").Append(unercqty).Append(", ");
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