﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using StarkBankExcel.Resources;
using System.Collections.Generic;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Interop.Excel;

namespace StarkBankExcel
{
    public partial class SendInvoices
    {
        private void Planilha5_Startup(object sender, System.EventArgs e)
        {
        }

        private void Planilha5_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region Código gerado pelo Designer VSTO

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InternalStartup()
        {
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button3.Click += new System.EventHandler(this.button3_Click);
            this.button4.Click += new System.EventHandler(this.button4_Click);
            this.button5.Click += new System.EventHandler(this.button5_Click);
            this.button6.Click += new System.EventHandler(this.button6_Click);
            this.Startup += new System.EventHandler(this.Planilha5_Startup);
            this.Shutdown += new System.EventHandler(this.Planilha5_Shutdown);

        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            var worksheet = Globals.SendInvoices;

            int lastRow = worksheet.Cells[worksheet.Rows.Count, "A"].End[XlDirection.xlUp].Row;

            worksheet.Range["A" + TableFormat.HeaderRow].Value = "Nome do Cliente";
            worksheet.Range["B" + TableFormat.HeaderRow].Value = "CPF/CNPJ do Cliente";
            worksheet.Range["C" + TableFormat.HeaderRow].Value = "Valor";
            worksheet.Range["D" + TableFormat.HeaderRow].Value = "Data de Vencimento";
            worksheet.Range["E" + TableFormat.HeaderRow].Value = "Multa";
            worksheet.Range["F" + TableFormat.HeaderRow].Value = "Juros ao Mês";
            worksheet.Range["G" + TableFormat.HeaderRow].Value = "Expiração em Horas";
            worksheet.Range["H" + TableFormat.HeaderRow].Value = "Descrição 1";
            worksheet.Range["I" + TableFormat.HeaderRow].Value = "Valor 1";
            worksheet.Range["J" + TableFormat.HeaderRow].Value = "Descrição 2";
            worksheet.Range["K" + TableFormat.HeaderRow].Value = "Valor 2";
            worksheet.Range["L" + TableFormat.HeaderRow].Value = "Descrição 3";
            worksheet.Range["M" + TableFormat.HeaderRow].Value = "Valor 3";

            string returnMessage = "";
            string warningMessage = "";
            string errorMessage = "";

            int iteration = 0;
            int errorNum = 10;

            var initRow = TableFormat.HeaderRow + 1;
            lastRow = worksheet.Cells[worksheet.Rows.Count, "A"].End[XlDirection.xlUp].Row;

            List<Dictionary<string, object>> invoices = new List<Dictionary<string, object>>();
            List<int> invoiceNumbers = new List<int>();

            for (int row = initRow; row <= lastRow; row++)
            {
                iteration++;

                string name = worksheet.Range["A" + row].Value?.ToString();
                string taxID = worksheet.Range["B" + row].Value?.ToString();
                string amountString = worksheet.Range["C" + row].Value?.ToString();
                int amount = Convert.ToInt32(double.Parse(amountString) * 100);
                string due = worksheet.Range["D" + row].Value?.ToString();
                string fineString = worksheet.Range["E" + row].Value?.ToString();
                string interestString = worksheet.Range["F" + row].Value?.ToString();
                string expirationString = worksheet.Range["G" + row].Value?.ToString().Replace(",", ".");

                List<Dictionary<string, string>> descriptions = new List<Dictionary<string, string>>();

                string description1 = worksheet.Range["H" + row].Value?.ToString();
                string value1 = worksheet.Range["I" + row].Value?.ToString();
                string description2 = worksheet.Range["J" + row].Value?.ToString();
                string value2 = worksheet.Range["K" + row].Value?.ToString();
                string description3 = worksheet.Range["L" + row].Value?.ToString();
                string value3 = worksheet.Range["M" + row].Value?.ToString();

                if(description1 != null && value1 != null)
                {
                    descriptions.Add(new Dictionary<string, string>
                    {
                        { "key", description1 },
                        { "value", value1 },
                    });
                }

                if (description2 != null && value2 != null)
                {
                    descriptions.Add(new Dictionary<string, string>
                    {
                        { "key", description2 },
                        { "value", value2 },
                    });
                }

                if (description3 != null && value3 != null)
                {
                    descriptions.Add(new Dictionary<string, string>
                    {
                        { "key", description3 },
                        { "value", value3 },
                    });
                }

                Dictionary<string, object> invoice = new Dictionary<string, object> {
                    { "amount", amount },
                    { "taxId", taxID },
                    { "name", name },
                    { "descriptions" , descriptions }
                };

                if (due != null) invoice.Add("due", new StarkDateTime(due).ToString());
                if (expirationString != null) invoice.Add("expiration", int.Parse(expirationString) * 3600);
                if (fineString != null) invoice.Add("fine", float.Parse(fineString));
                if (interestString != null) invoice.Add("interest", float.Parse(interestString));

                invoiceNumbers.Add(iteration);

                invoices.Add(invoice);

                if (iteration % 100 == 0 || row >= lastRow)
                {
                    if (invoiceNumbers.Count == 0) goto nextIteration;

                    try
                    {
                        JObject res = Invoice.Create(invoices);

                        string createInvoice = (string)res["message"];
                        returnMessage = returnMessage + Utils.rowsMessage(initRow, row) + createInvoice + "\n";
                    }
                    catch (Exception ex)
                    {
                        errorMessage = Utils.ParsingErrors(ex.Message, errorNum);
                    }

                errorNum += 100;

                nextIteration:
                    initRow = row + 1;
                    invoices = new List<Dictionary<string, object>>();
                    invoiceNumbers = new List<int>();
                }
            }

            MessageBox.Show(warningMessage + returnMessage + errorMessage);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Globals.Main.Activate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Utils.LogOut();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var worksheet = Globals.SendInvoices;

            Range range = worksheet.Range["A" + (TableFormat.HeaderRow + 1) + ":K1048576"];
            range.ClearContents();
        }
    }
}
