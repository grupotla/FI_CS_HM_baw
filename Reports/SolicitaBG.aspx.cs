using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_SolicitaER_2 : System.Web.UI.Page
{
    UsuarioBean user = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usuario"] == null) {
            Response.Redirect("../default.aspx");
        }
        user = (UsuarioBean)Session["usuario"];

        if (!Page.IsPostBack)
        {
            obtengo_lista();
            //************************RESTRICCION DE CONTABILIDAD USUARIOS****************************//
            lb_contabilidad.Items.Clear();
            int bandera = 0; // verifica si el usuario tiener contabilidad consolidada.
            int fiscal = 0;
            int financiera = 0;
            ListItem item = null;
            ArrayList arruser = (ArrayList)DB.getContaUsuario(user.ID, user.PaisID);
            ArrayList arrbloqueo = (ArrayList)DB.getContaPais(user.PaisID);
            foreach (RE_GenericBean rgbp in arrbloqueo)
            {
                if (rgbp.intC1 == 1)
                {
                    fiscal = 1; //desbloqueo fiscal
                }

                if (rgbp.intC2 == 1)
                {
                    financiera = 1; // desbloqueo financiera.
                }
            }
            foreach (RE_GenericBean rgb in arruser)
            {
                if ((rgb.intC1 == 1) && (fiscal == 1))
                {
                    bandera++;
                    item = new ListItem("FISCAL", "1");
                    if (user.contaID == 1)
                    {
                        lb_contabilidad.Items.Add(item);
                    }
                }

                if ((rgb.intC2 == 1) && (financiera == 1))
                {
                    bandera++;
                    item = new ListItem("FINANCIERA", "2");
                    if (user.contaID == 2)
                    {
                        lb_contabilidad.Items.Add(item);
                    }
                }
            }
            if (bandera == 2)
            {
                item = new ListItem("CONSOLIDADO", "3");
                lb_contabilidad.Items.Add(item);
            }
            if (bandera == 1)
            {
                lb_contabilidad.Visible = false;
                l_contabilidad.Visible = false;
            }
            else
            {
                lb_contabilidad.Visible = true;
                l_contabilidad.Visible = true;
            }
            if (user.contaID == 1)
            {
                chk_consolidado.Visible = true;
                l_moneda_consolidado.Visible = true;
            }
            if (user.contaID == 2)
            {
                chk_consolidado.Visible = false;
                l_moneda_consolidado.Visible = false;
            }

            //*********************************FIN RESTRICCION********************************************//
        }
    
    }

    protected void obtengo_lista()
    {
        ArrayList arr = null;
        ListItem item = null;
        user = (UsuarioBean)Session["usuario"];
        arr = (ArrayList)DB.getMonedasbyPais(user.PaisID, user.contaID);
        foreach (RE_GenericBean rgb in arr)
        {
            item = new ListItem(rgb.strC1, rgb.intC1.ToString());
            lb_moneda.Items.Add(item);
        }
    }
    protected void btn_generar_Click(object sender, EventArgs e)
    {
        string mensaje = "";
        decimal tp = 0;
        string consmonfiscal = "";
        if (chk_consolidado.Checked == true)
        {
            consmonfiscal = "si";
        }
        if (chk_consolidado.Checked == false)
        {
            consmonfiscal = "no";
        }

        if (lb_tipo_reporte.SelectedValue == "1")
        {
            
            if (tb_fechacorte.Text.Trim() == "")
            {
                WebMsgBox.Show("Debe Ingresar la Fecha de Corte");
                return;
            }         
            else
            {
                tp = DB.getTipoCambioByDay(user,  DB.DateFormat(tb_fechacorte.Text.Trim()));
                if ((tp == 0) && (lb_contabilidad.SelectedValue == "3"))
                {
                    WebMsgBox.Show("No existe tipo de cambio para esta fecha de corte: " + tb_fechacorte.Text.Trim());
                    return;
                }

                #region Registrar Log de Reportes
                RE_GenericBean Bean_Log = new RE_GenericBean();
                Bean_Log.intC1 = user.PaisID;
                Bean_Log.strC1 = "BALANCE GENERAL";
                Bean_Log.strC2 = user.ID;
                Bean_Log.strC3 = Request.ServerVariables["REMOTE_ADDR"].ToString();
                string mensaje_log = "";
                mensaje_log += "Tipo Reporte.: Acumulado, ";
                mensaje_log += "Moneda.: " + lb_moneda.SelectedItem.Text + " Contabilidad.: " + lb_contabilidad.SelectedItem.Text + " ,";
                mensaje_log += "Fecha Inicial.: " + tb_fechainicial.Text + " Fecha Final.: " + tb_fechafinal.Text + " ,";
                mensaje_log += "Consolidar Moneda.: " + consmonfiscal + " ";
                Bean_Log.strC4 = mensaje_log;
                DB.Insertar_Log_Reportes(Bean_Log);
                #endregion

                if (regional.Checked == true)
                {
                    mensaje = "<script languaje=\"JavaScript\">";
                    mensaje += "window.open('BGRegional.aspx?t_rep=" + lb_tipo_reporte.SelectedValue + "&monID=" + lb_moneda.SelectedValue + "&tcon=" + lb_contabilidad.SelectedValue + "&f_corte=" + tb_fechacorte.Text.Trim() + "&consmonfiscal=" + consmonfiscal + "','Genera_Reporte','menubar=1,resizable=1,scrollbars=1,toolbar=1,width=750');";
                    mensaje += "</script>";
                    Page.RegisterClientScriptBlock("closewindow", mensaje);

                }
                else
                {
                    mensaje = "<script languaje=\"JavaScript\">";
                    mensaje += "window.open('balance_general.aspx?t_rep=" + lb_tipo_reporte.SelectedValue + "&monID=" + lb_moneda.SelectedValue + "&tcon=" + lb_contabilidad.SelectedValue + "&f_corte=" + tb_fechacorte.Text.Trim() + "&consmonfiscal=" + consmonfiscal + "','Genera_Reporte','menubar=1,resizable=1,scrollbars=1,toolbar=1,width=750');";
                    mensaje += "</script>";
                    Page.RegisterClientScriptBlock("closewindow", mensaje);
                }
            }
        }
        else if (lb_tipo_reporte.SelectedValue == "2")
        {
            if ((tb_fechainicial.Text.Trim() == "") || (tb_fechafinal.Text.Trim() == ""))
            {
                if (tb_fechainicial.Text.Trim() == "")
                {
                    WebMsgBox.Show("Debe Ingresar la Fecha Inicial");
                    return;
                }
                if (tb_fechafinal.Text.Trim() == "")
                {
                    WebMsgBox.Show("Debe Ingresar la Fecha Final");
                    return;
                }
            }
            else
            {
                tp = DB.getTipoCambioByDay(user, DB.DateFormat(tb_fechafinal.Text.Trim()));
                if ((tp == 0) && (lb_contabilidad.SelectedValue == "3"))
                {
                    WebMsgBox.Show("No existe tipo de cambio para esta fecha de corte: " + tb_fechafinal.Text.Trim());
                    return;
                }

                #region Registrar Log de Reportes
                RE_GenericBean Bean_Log = new RE_GenericBean();
                Bean_Log.intC1 = user.PaisID;
                Bean_Log.strC1 = "BALANCE GENERAL";
                Bean_Log.strC2 = user.ID;
                Bean_Log.strC3 = Request.ServerVariables["REMOTE_ADDR"].ToString();
                string mensaje_log = "";
                mensaje_log += "Tipo Reporte.: Acotado, ";
                mensaje_log += "Moneda.: " + lb_moneda.SelectedItem.Text + " Contabilidad.: " + lb_contabilidad.SelectedItem.Text + " ,";
                mensaje_log += "Fecha Inicial.: " + tb_fechainicial.Text + " Fecha Final.: " + tb_fechafinal.Text + " ,";
                mensaje_log += "Consolidar Moneda.: " + consmonfiscal + " ";
                Bean_Log.strC4 = mensaje_log;
                DB.Insertar_Log_Reportes(Bean_Log);
                #endregion

                if (regional.Checked == true)
                {
                    mensaje = "<script languaje=\"JavaScript\">";
                    mensaje += "window.open('BGRegional.aspx?t_rep=" + lb_tipo_reporte.SelectedValue + "&monID=" + lb_moneda.SelectedValue + "&tcon=" + lb_contabilidad.SelectedValue + "&f_inicial=" + tb_fechainicial.Text.Trim() + "&f_final=" + tb_fechafinal.Text.Trim() + "&consmonfiscal=" + consmonfiscal + "','Genera_Reporte','menubar=1,resizable=1,scrollbars=1,toolbar=1,width=750');";
                    mensaje += "</script>";
                    Page.RegisterClientScriptBlock("closewindow", mensaje);
                }
                else
                {
                    mensaje = "<script languaje=\"JavaScript\">";
                    mensaje += "window.open('balance_general.aspx?t_rep=" + lb_tipo_reporte.SelectedValue + "&monID=" + lb_moneda.SelectedValue + "&tcon=" + lb_contabilidad.SelectedValue + "&f_inicial=" + tb_fechainicial.Text.Trim() + "&f_final=" + tb_fechafinal.Text.Trim() + "&consmonfiscal=" + consmonfiscal + "','Genera_Reporte','menubar=1,resizable=1,scrollbars=1,toolbar=1,width=750');";
                    mensaje += "</script>";
                    Page.RegisterClientScriptBlock("closewindow", mensaje);
 
                }
            }
        }
        else if (lb_tipo_reporte.SelectedValue == "0")
        {
            WebMsgBox.Show("Debe Seleccionar el Tipo de Reporte a Generar");
            return;
        }
    }
    protected void lb_tipo_reporte_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lb_tipo_reporte.SelectedValue == "0")
        {
            Panel1.Visible = false;
            Panel2.Visible = false;
        }
        else if (lb_tipo_reporte.SelectedValue == "1")
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
        }
        else if (lb_tipo_reporte.SelectedValue == "2")
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
        }
    }

    protected void lb_contabilidad_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lb_contabilidad.SelectedValue == "3")
        {
            chk_consolidado.Checked = false;
            chk_consolidado.Visible = false;
            l_moneda_consolidado.Visible = false;
        }
        if (lb_contabilidad.SelectedValue == "1")
        {
            chk_consolidado.Visible = true;
            l_moneda_consolidado.Visible = true;
        }
    } 
}
