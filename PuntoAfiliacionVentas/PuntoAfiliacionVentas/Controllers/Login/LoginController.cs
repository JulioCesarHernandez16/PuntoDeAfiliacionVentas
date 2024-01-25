using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PuntoAfiliacionVentas.Controllers.Login
{
    public class LoginController : Controller
    {
        // GET: Login
        Dictionary<string, object> Arr_Insert = new Dictionary<string, object>();
        Dictionary<string, object> Arr_Where = new Dictionary<string, object>();
        Dictionary<string, object> Arr_Update = new Dictionary<string, object>();
        // GET: CargoAutomatico

        /*Operadores Where*/
        String[] Operadores = new String[2];
        String[] Operadores2 = new String[2];


        PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL MYSQL_C = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();
        /*Fin Operadores Where*/
        public ActionResult Login()
        {
            return View();
        }

        public async Task<ActionResult> Logueo(string txtEmail, string txtPassword)
        {
            string mensaeje = "Error";
            try
            {
                
                string select = "Select * from punto_afiliacion_comercios where correo_electronico = '" + txtEmail + "' and Contrasenna = '" + txtPassword + "' and ESTADO = '1' ";
                DataTable Logueo = MYSQL_C.MYSQLSelect("BILLINGMYSQL_1", select);
                if (Logueo.Rows.Count > 0)
                {
                    Session["Comercio"] = Logueo.Rows[0]["ID"].ToString().Trim();
                    Session["NombreComercio"] = Logueo.Rows[0]["COMERCIO"].ToString().Trim();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Session["Comercio"] = null;
                    TempData["LoginError"] = "Contraseña o usuario invalido.";
                    return RedirectToAction("Login", "Login");
                }

            }
            catch (Exception e)
            {
                TempData["LoginError"] = e.Message;
                throw;
            }


        }



    }
}