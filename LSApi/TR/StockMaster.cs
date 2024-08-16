using System.Text;
using XA_DATASETLib;

namespace LSApi.TR
{
    internal class StockMaster
    {
        #region [t8436] Request and receive stock master
        static XAQueryClass t8436;
        public static void ReqStockMaster()
        {
            t8436 = new XAQueryClass();
            t8436.ResFileName = App.IConf["LS:Res"] + "t8436.res";
            t8436.ReceiveData += RcvT8436;

            t8436.SetFieldData("t8436InBlock", "gubun", 0, "0");
            var result = t8436.Request(false);

            if (result < 0) Program.ILog.Warning("[t8436] Failed stock master inquiry");
        }

        static void RcvT8436(string tr)
        {
            var query = new StringBuilder();

            string trout = tr + "OutBlock";
            for (int i = 0; i < t8436.GetBlockCount(trout); i++)
            {
                // Exclude non-securities group stocks
                if (t8436.GetFieldData(trout, "bu12gubun", i) != "01") continue;
                // Exclude SPAC
                if (t8436.GetFieldData(trout, "spac_gubun", i) == "Y") continue;


                var code    = t8436.GetFieldData(trout, "shcode", i);   // Stock short code
                var name    = t8436.GetFieldData(trout, "hname", i);    // Stock name
                var market  = t8436.GetFieldData(trout, "gubun", i);
                var close   = decimal.Parse(t8436.GetFieldData(trout, "jnilclose", i)); // Previous closing price

                App.Stocks[code] = new Stock 
                {
                    Code        = code,
                    Name        = name,
                    Market      = market == "1" ? Exchange.KOSPI : Exchange.KOSDAQ,
                    JunilClose  = close
                };

                query.AppendLine($"INSERT INTO tb_stock_items (code, name, market) 
                    VALUES ('{code}', '{name}', '{market}') ON DUPLICATE KEY UPDATE name='{name}', market='{market}';");

                App.TradingItems.TryGetValue(code, out Item im);
                if (im != null)
                {
                    im.Market       = market == "1" ? Exchange.KOSPI : Exchange.KOSDAQ;
                    im.JunilClose   = close;
                }
            }

            Program.ILog.Information("[t8436] Finish stock master inquiry");
            DB.Execute(query.ToString());
            
            Program.Waiting?.TrySetResult(true);
        }
        #endregion
    }
}