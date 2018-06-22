using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.DataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace LaboratorioExames
{
    public partial class ListagemExames : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarExames();
            }
        }

        private void CarregarExames()
        {
            OracleConnection con = ConectarAoBanco();

            var controlDe = this.FindControl("txtDe") as TextBox;
            var controlAte = this.FindControl("txtAte") as TextBox;

            DateTime dataDe = DateTime.Today;
            DateTime dataAte = dataDe.AddDays(1);

            if (!String.IsNullOrEmpty(controlDe.Text))
            {
                dataDe = DateTime.Parse(controlDe.Text, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (!String.IsNullOrEmpty(controlAte.Text))
            {
                dataAte = DateTime.Parse(controlAte.Text, System.Globalization.CultureInfo.InvariantCulture);
            }

            DataTable dt = new DataTable();
            DataRow dr = null;

            dt.Columns.Add(new DataColumn("PORC", typeof(int)));
            dt.Columns.Add(new DataColumn("STAT", typeof(string)));
            dt.Columns.Add(new DataColumn("DH", typeof(string)));
            dt.Columns.Add(new DataColumn("MEDIA", typeof(string)));
            dt.Columns.Add(new DataColumn("PREV", typeof(string)));
            dt.Columns.Add(new DataColumn("TEMP", typeof(string)));
            dt.Columns.Add(new DataColumn("PED", typeof(int)));
            dt.Columns.Add(new DataColumn("ATEND", typeof(int)));
            dt.Columns.Add(new DataColumn("PAC", typeof(string)));
            dt.Columns.Add(new DataColumn("SET", typeof(string)));
            dt.Columns.Add(new DataColumn("EXA", typeof(string)));
            dt.Columns.Add(new DataColumn("USU", typeof(string)));

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = con;
            cmd.CommandText = "SELECT DISTINCT exa_lab.vl_tempo_medio, ped_lab.dt_pedido, ped_lab.hr_ped_lab, ped_lab.cd_ped_lab, ped_lab.cd_atendimento, paciente.nm_paciente, setor.nm_setor, exa_lab.nm_exa_lab, amostra.cd_usuario_coleta " +
                              "FROM ped_lab, itped_lab, exa_lab, atendime, paciente, setor, coleta_material, amostra " +
                              "WHERE itped_lab.dt_assinado IS NULL " +
                              "AND itped_lab.cd_exa_lab = exa_lab.cd_exa_lab AND itped_lab.cd_ped_lab = ped_lab.cd_ped_lab AND atendime.cd_atendimento = ped_lab.cd_atendimento AND atendime.cd_paciente = paciente.cd_paciente AND ped_lab.cd_setor = setor.cd_setor " +
                              "AND itped_lab.cd_set_exa = 1 AND ped_lab.cd_ped_lab = coleta_material.cd_ped_lab AND coleta_material.cd_coleta_material = amostra.cd_coleta_material " +
                              "AND setor.cd_setor IN (135, 136, 165, 147, 148) " + // 41 e 42 = UCO e UTIN -> retirados momentaneamente
                              "AND ped_lab.dt_pedido BETWEEN TO_DATE('" + dataDe.ToString().Substring(0, 10) + "', 'dd/MM/yyyy') AND TO_DATE('" + dataAte.ToString().Substring(0, 10) + "', 'dd/MM/yyyy') " +
                              "ORDER BY ped_lab.hr_ped_lab ";
            cmd.CommandType = CommandType.Text;
            OracleDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                if (dataReader.IsDBNull(0))
                    continue;

                dr = dt.NewRow();

                //dr["MEDIA"] = "00:00";
                string media = dataReader.GetDateTime(0).ToShortTimeString();
                dr["MEDIA"] = media;

                if (!dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                {
                    string dh_data = dataReader.GetDateTime(1).ToShortDateString();
                    string dh_hora = dataReader.GetDateTime(2).ToShortTimeString();

                    TimeSpan prev_hora = TimeSpan.Parse(dh_hora) + TimeSpan.Parse(media);
                    DateTime prev = DateTime.Parse(dh_data) + prev_hora;

                    TimeSpan _temp = prev - DateTime.Now;
                    string temp = "00:00";
                    if (_temp.Hours > 0 || _temp.Minutes > 0)
                        temp = (_temp.Hours.ToString().Length == 1 ? "0" : "") + _temp.Hours + ":" + (_temp.Minutes.ToString().Length == 1 ? "0" : "") + _temp.Minutes;

                    int porcem = (((_temp.Hours * 60 * 60) + (_temp.Minutes * 60)) * 100) / ((int.Parse(media.Substring(0, 2)) * 60 * 60) + (int.Parse(media.Substring(3, 2)) * 60));

                    //if (porcem < 0)
                    //    porcem = 0;

                    dr["DH"] = dh_data + " " + dh_hora;
                    dr["PREV"] = prev.ToShortDateString() + " " + prev.ToShortTimeString();
                    dr["TEMP"] = temp;

                    if (dataReader.IsDBNull(8))
                    {
                        dr["STAT"] = "👎";
                        dr["USU"] = " ";
                    }
                    else
                    {
                        dr["STAT"] = "👍";
                        dr["USU"] = dataReader.GetString(8);
                    }

                    dr["PORC"] = porcem;
                }
                if (!dataReader.IsDBNull(3))
                    dr["PED"] = dataReader.GetInt32(3);
                if (!dataReader.IsDBNull(4))
                    dr["ATEND"] = dataReader.GetInt32(4);
                if (!dataReader.IsDBNull(5))
                {
                    string paciente = dataReader.GetString(5);
                    if (paciente.Length > 25)
                        paciente = paciente.Substring(0, 25);
                    dr["PAC"] = paciente;
                }
                if (!dataReader.IsDBNull(6))
                {
                    string setor = dataReader.GetString(6);
                    if (setor == "PRONTO SOCORRO INTERNAÇÃO")
                        setor = "PS INTERNAÇÃO";
                    //else if (setor == "TERAPIA INTEN. CORONARIANA")
                    //    setor = "UCO";
                    else if (setor == "TERAPIA INTEN. GERAL I")
                        setor = "CTI I";
                    else if (setor == "TERAPIA INTEN. GERAL II")
                        setor = "CTI II";
                    else if (setor == "PRONTO SOCORRO URGÊNCIA")
                        setor = "PS URGÊNCIA";
                    //else if (setor == "TERAPIA INTEN. NEONATAL")
                    //    setor = "UTIN";
                    else if (setor == "PRONTO ATENDIMENTO CONVÊNIO")
                        setor = "PA CONVÊNIO";
                    dr["SET"] = setor;
                }
                if (!dataReader.IsDBNull(7))
                    dr["EXA"] = dataReader.GetString(7);

                dt.Rows.Add(dr);
            }

            var controlGrid = this.FindControl("grdExames") as GridView;

            controlGrid.DataSource = dt;
            controlGrid.DataBind();

            FecharConexaoBanco(con);
        }

        private OracleConnection ConectarAoBanco()
        {
            OracleConnection con = new OracleConnection();
            con.ConnectionString = "xxxx";
            con.Open();
            return con;
        }

        private void FecharConexaoBanco(OracleConnection con)
        {
            con.Close();
            con.Dispose();
        }

        protected void btnPesquisa_Click(object sender, EventArgs e)
        {
            CarregarExames();
        }

        protected void grdExames_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowIndex >= 0)
            {
                DateTime data = DateTime.Parse(e.Row.Cells[2].Text);
                int porcem = int.Parse(e.Row.Cells[0].Text);

                if (data.Date < DateTime.Now.Date)
                    porcem = -999;
                
                if (porcem > 0) // Verde
                {
                    e.Row.Cells[1].BackColor = System.Drawing.ColorTranslator.FromHtml("#2e9b46");
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#2e9b46");
                    e.Row.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("#2e9b46");

                    e.Row.Cells[1].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                    e.Row.Cells[2].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                    e.Row.Cells[3].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                }
                else if (porcem >= -20 && porcem <= 0) // Amarelo
                {
                    e.Row.Cells[1].BackColor = System.Drawing.ColorTranslator.FromHtml("#e6b800");
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#e6b800");
                    e.Row.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("#e6b800");

                    e.Row.Cells[1].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                    e.Row.Cells[2].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                    e.Row.Cells[3].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                }
                else // Vermelho
                {
                    e.Row.Cells[1].BackColor = System.Drawing.ColorTranslator.FromHtml("#a83a3a");
                    e.Row.Cells[2].BackColor = System.Drawing.ColorTranslator.FromHtml("#a83a3a");
                    e.Row.Cells[3].BackColor = System.Drawing.ColorTranslator.FromHtml("#a83a3a");

                    e.Row.Cells[1].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                    e.Row.Cells[2].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                    e.Row.Cells[3].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
                }

                e.Row.Cells[1].ToolTip = e.Row.Cells[11].Text;
            }
        }
    }
}