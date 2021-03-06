using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class manager_addsuc : System.Web.UI.Page
{
    SucursalBean sucursal = null;
    public int id = 0;
    public ArrayList fac_arr = null;
    public ArrayList rec_arr = null;
    public ArrayList nc_arr = null;
    public ArrayList nd_arr = null;
    public ArrayList ncnd_arr = null;
    public ArrayList oc_arr = null;
    public ArrayList cf_arr = null;
    public ArrayList pa_arr = null;
    public ArrayList ecp_arr = null;
    public ArrayList ret_arr = null;
    public ArrayList ncp_arr = null;
    public ArrayList prof_arr = null;
    public ArrayList ajcon_arr = null;
    public ArrayList ajconNC_arr = null;
    public ArrayList rec_corte_arr = null;
    public ArrayList Liq_arr = null;
   
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["usuario"] == null)
        {
            Session["msg"] = "Usted debe generar una nueva sesion";
            Response.Redirect("../Default.aspx");
        }
        UsuarioBean user = (UsuarioBean)Session["usuario"];
        ArrayList Arr_Perfiles = (ArrayList)DB.getPerfilesxUsu(user.ID, user.PaisID);
        int Flag = 0;
        foreach (PerfilesBean Perfil in Arr_Perfiles)
        {
            if ((Perfil.ID == 8) || (Perfil.ID == 19))
            {
                Flag++;
            }
        }
        if (Flag == 0)
        {
            Response.Redirect("../operations/index.aspx");
        }

        ArrayList paises_arr = (ArrayList)DB.getPaises("");
        ListItem item=null;
        PaisBean paisbean=new PaisBean();
        
        for (int i = 0; i < paises_arr.Count; i++) 
        { 
            paisbean=(PaisBean)paises_arr[i];
            item=new ListItem(paisbean.Nombre, paisbean.ID.ToString());
        }

        if (!Page.IsPostBack)
        {
            if (Request.QueryString["id"] != null)
            {
                id = int.Parse(Request.QueryString["id"].ToString());
                sucursal = (SucursalBean)DB.getSucursal(id);
                if (sucursal == null)
                {
                    Session["msg"] = "Error, No se pudieron obtener los datos de este registro, por favor intente de nuevo.";
                    Session["url"] = "addpais.aspx?id=" + id;
                    Response.Redirect("message.aspx");
                }
                tb_nombre.Text = sucursal.Nombre;
                fac_arr = (ArrayList)DB.getFacturasbysuc(id, 1); //obtiene las facturas asociadas
                rec_arr = (ArrayList)DB.getFacturasbysuc(id, 2); //Obtiene los recibos asociados
                nc_arr = (ArrayList)DB.getFacturasbysuc(id, 3); //Obtiene las notas de credito
                nd_arr = (ArrayList)DB.getFacturasbysuc(id, 4); //Obtiene las notas de debito asociadas
                pa_arr = (ArrayList)DB.getFacturasbysuc(id, 5); //Provision Agente
                oc_arr = (ArrayList)DB.getFacturasbysuc(id, 7); //Obtiene las ordenes de compra
                cf_arr = (ArrayList)DB.getFacturasbysuc(id, 8); //Obtiene las contraseña facturas
                ret_arr = (ArrayList)DB.getFacturasbysuc(id, 20); //Obtiene las contraseña de retenciones
                
                ecp_arr = (ArrayList)DB.getFacturasbysuc(id, 11); //serie de corte de proveedores
                ncp_arr = (ArrayList)DB.getFacturasbysuc(id, 12); //serie nota credito proveedores
                ncnd_arr = (ArrayList)DB.getFacturasbysuc(id, 31); //serie nota credito proveedores
                prof_arr = (ArrayList)DB.getFacturasbysuc(id, 14); //Serie de facturas proforma
                ajcon_arr = (ArrayList)DB.getFacturasbysuc(id, 15); //Serie de facturas proforma
                ajconNC_arr = (ArrayList)DB.getFacturasbysuc(id, 18); //Serie de ajustes contables nota credito
                rec_corte_arr = (ArrayList)DB.getFacturasbysuc(id, 22); //Serie de recibos de Cortes
                Liq_arr = (ArrayList)DB.getFacturasbysuc(id, 24); //Serie de recibos de Cortes
            }
            else {
                tb_nombre.Text = "";
            }
        }

    }
}
