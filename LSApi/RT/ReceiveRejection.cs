using System.Text;
using XA_DATASETLib;

namespace LSApi.RT
{
    internal class ReceiveRejection
    {
        XARealClass xa;
        public ReceiveRejection()
        {
            xa = new XARealClass();
            xa.ResFileName = App.IConf["Ebest:Res"] + "SC4.res";
            xa.ReceiveRealData += ReceiveMessage;
            xa.AdviseRealData();
        }

        public void ReceiveMessage(string tr)
        {
            try
            {
                //if (xa.GetFieldData("OutBlock", "accno") != Conf.IConfig["Ebest:Acnt"]) return;

                string code = xa.GetFieldData("OutBlock", "shtnIsuno").Substring(1).Trim();        // Stock Ticker
                App.TradingItems.TryGetValue(code, out Item im);
                if (im == null) return;

                long rjt_qty    = long.Parse(xa.GetFieldData("OutBlock", "rjtqty"));               // Rejected Quantity
                string bs       = xa.GetFieldData("OutBlock", "bnstp");                            // 1:ASK, 2:BID
                long ord_qty    = long.Parse(xa.GetFieldData("OutBlock", "ordqty"));               // Order Quantity
                long ord_prc    = long.Parse(xa.GetFieldData("OutBlock", "ordprc"));               // Order Price
                string ordno    = xa.GetFieldData("OutBlock", "ordno");                            // Order NO             
                string orgordno = xa.GetFieldData("OutBlock", "orgordno");                         // Original Order NO
                string trcode   = xa.GetFieldData("OutBlock", "trcode");                           // TRCODE

                //StringBuilder sb = new StringBuilder();
                //sb.Append(Dic.RejectTrCode[trcode]).Append("Rejection").Append(", ");
                //sb.Append("code:").Append(code).Append(", ");
                //sb.Append("name:").Append(im.Name).Append(", ");
                //sb.Append("side:").Append(Dic.BuySell[bs]).Append(", ");
                //sb.Append("ordno:").Append(ordno).Append(", ");
                //sb.Append("orgordno:").Append(orgordno).Append(", ");
                //sb.Append("ordqty:").Append(ord_qty).Append(", ");
                //sb.Append("ordprc:").Append(ord_prc).Append(", ");
                //sb.Append("rjtqty:").Append(rjt_qty).Append(", ");
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