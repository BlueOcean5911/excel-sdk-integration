﻿using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StarkBankExcel.Resources
{
    internal class Invoice
    {
        public static JObject Create(List<Dictionary<string, object>> payloads)
        {
            Dictionary<string, object> body = new Dictionary<string, object>
            {
                { "invoices", payloads }
            };

            return Request.Fetch(
                Request.Post,
                Globals.Credentials.Range["B3"].Value,
                "invoice",
                body
            ).ToJson();
        }

        public static JObject Get(string cursor = null, Dictionary<string, object> optionalParams = null)
        {
            string query = "";

            if (cursor != null)
            {
                query = "?cursor=" + cursor;
            }

            if (optionalParams != null)
            {
                foreach (string key in optionalParams.Keys)
                {
                    if (query == "")
                    {
                        query = "?" + key + "=" + optionalParams[key].ToString();
                    }
                    else
                    {
                        query = query + "&" + key + "=" + optionalParams[key].ToString();
                    }
                }
            }

            return Request.Fetch(
                Request.Get,
                Globals.Credentials.Range["B3"].Value,
                "invoice/" + query
            ).ToJson();
        }

        public static JObject Update(string id, Dictionary<string, object> payload)
        {

            return Request.Fetch(
                Request.Patch,
                Globals.Credentials.Range["B3"].Value,
                "invoice/" + id,
                payload
            ).ToJson();
        }

    }
}
