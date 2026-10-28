using Hydra;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System;

[assembly: CallbackAssemblyDescription(Name: "ONE-026 E-mail",
                                           Description: "Funkcja sprawdzająca czy  podczas zakładania nowego kontrahenta wpisany przez przedstawiciela handlowego adres e-mail już figuruje w bazie danych przypisany do innego kontrahenta. "
                                                        +" Komunikat pojawia się po wypełnieniu pola E - Mail, w momencie wychodzenia z pola E - Mail. Komunikat ma formę informacyjną i nie uniemożliwia kontynuowania operacji dodawania nowego kontrahenta.",
                                           Author: "OneSupport",
                                           Version: "1.0",
                                           SystemVersion: "2022.0.2",
                                           Created: "25-05-2023")]
namespace Przycisk
{
    [SubscribeProcedure(Procedures.KntEdycja, "E-mail")]
    public class DodajFakture : Callback
    {
        private ClaWindow Email;
        private ClaWindow Akronim;
       // private Dane dane;
        public override void Init()
        {
            AddSubscription(true, 0, Events.OpenWindow, new TakeEventDelegate(OnOpenWindow));
            AddSubscription(false, 0, Events.ResizeWindow, new TakeEventDelegate(Resize));
        }



        private bool OnOpenWindow(Procedures ProcId, int ControlId, Events Event)
        {
            
            ClaWindow Parent = GetWindow();
            Rectangle WndBnd = GetWindow().Bounds;
            //zakladkaTabWewnetrzne = GetWindow().AllChildren["?TabFS"];

            Email= Parent.AllChildren["?Knt:EMail"];
            Akronim = Parent.AllChildren["?Knt:Akronim"];
            AddSubscription(false, Email.Id, Events.Accepted, new TakeEventDelegate(OnSave));


            //listID = GetWindow().AllChildren["?Handlowe"].Id;      //tu sprawdzic

            //pobrane z pola w zakładce Raporty
            return true;
        }

        public bool Resize(Procedures ProcId, int ControlId, Events Event)
        {
            
            return true;
        }

        public bool OnSave(Procedures ProcId, int ControlId, Events Event)
        {
            if (!String.IsNullOrEmpty(Email.ValueRaw) && !(String.IsNullOrEmpty(Akronim.ValueRaw)))
            {
                string connectionString = @"Persist Security Info=False;Integrated Security=False;UID=sabisup;password=#0ut2510gro0ne.34;database=" + Runtime.Config.Baza + ";server=" + Runtime.Config.Serwer + ";Connect Timeout=30";
                if (ConnectToSQL(connectionString, "if exists(SELECT * FROM cdn.KntKarty where Knt_Email='" + Email.ValueRaw.ToString() + "' and Knt_Akronim !='" + Akronim.ValueRaw.ToString() + "') SELECT 'TAK' else SELECT 'NIE'") == "TAK")
                {
                    string akronim = ConnectToSQL(connectionString, "SELECT TOP (1) Knt_Akronim FROM cdn.KntKarty where Knt_Email='" + Email.ValueRaw.ToString() + "' and Knt_Akronim !='" + Akronim.ValueRaw.ToString() + "'");
                    MessageBox.Show("Ten adres e - mail jest już przypisany do " + akronim, "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    

                }
            }
            //MessageBox.Show("Email");
            return true;
        }

        public string ConnectToSQL(string connection_string, string query)
        {
            string str = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Query");
                    if (dataSet.Tables.Count == 0)
                    {
                        return "";
                    }
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        str = row[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                str = str + "Bład SQL:" + ex.ToString() + "\n";
            }
            return str;
        }

        private bool BtnOnMouseDown(Procedures ProcId, int ControlId, Events Event)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 9) == "ERPXL_E_P")
            {
               // MessageBox.Show("Po wybraniu pliku nastąpi dodawanie danych do bazy ERPXL_E_P", "Komunikat",
                // MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start("C:\\OneSupport\\ImportDekretow\\Test\\ImportDekretow.exe");
            }
            else if (Runtime.ConfigurationDictionary.Baza.ToString().Substring(0, 7) == "ERPXL_E")
            {
                //MessageBox.Show("Po wybraniu pliku nastąpi dodawanie danych do bazy ERPXL_E", "Komunikat",
                 //MessageBoxButtons.OK, MessageBoxIcon.Information);
                Process.Start("C:\\OneSupport\\ImportDekretow\\Produkcja\\ImportDekretow.exe");
            }
            

            return true;
        }
        #region
        public override void Cleanup()
        {
        }
    }
}

#endregion