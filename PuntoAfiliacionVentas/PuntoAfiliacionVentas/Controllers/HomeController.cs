using Newtonsoft.Json;
using PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL;
using PuntoAfiliacionVentas.ComponenteSQL.Trydicionary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntoAfiliacionVentas.Controllers
{


    public class HomeController : Controller
    {
        Dictionary<string, object> Arr_Insert = new Dictionary<string, object>();
        Dictionary<string, object> Arr_Where = new Dictionary<string, object>();
        Dictionary<string, object> Arr_Update = new Dictionary<string, object>();
        Trictionary<string, object, string[]> Arr_Where_ = new Trictionary<string, object, string[]>(); //El trictionary se utiliza para los where
        // GET: CargoAutomatico

        /*Operadores Where*/
        String[] Operadores = new String[2];
        String[] Operadores2 = new String[2];
        /*Fin Operadores Where*/


        /*Componente SQL*/

        PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL MYSQL_C = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();
        /*Fin Directas*/

        /*Fin Componente*/

        public ActionResult Index()
        {
            try
            {
                if (Session["Comercio"] == null)
                {
                    return View("~/Views/Login/Login.cshtml");
                }
                ViewBag.NombreComercio = Session["NombreComercio"].ToString().Trim();
            }
            catch (Exception)
            {
                return View("Login/Login");
            }
            return View();
        }
        public ActionResult InsertaLead(string Nombre, string Correo, string Telefono1, string Telefono2, string Direccion, string ServiciosInteres)
        {
            try
            {

                if (Session["Comercio"] == null)
                {
                    return View("~/Views/Login/Login.cshtml");
                }

                string IDComercio = Session["Comercio"].ToString().Trim(); //SESSSION
                                                                           //string IDComercio = Session["Comercio"].ToString().Trim(); //SESSSION
                string Estado = "1";
                string UsuarioAsignado = "";
                DateTime FechaCreacion = DateTime.Now;
                Arr_Insert.Clear();
                Arr_Insert.Add("NOMBRE_COMPLETO", Nombre);
                Arr_Insert.Add("CORREO", Correo);
                Arr_Insert.Add("TELEFONO1", Telefono1);
                if (!string.IsNullOrEmpty(Telefono2))
                {
                    Arr_Insert.Add("TELEFONO2", Telefono2);
                }
                Arr_Insert.Add("DIRECCION", Direccion);
                Arr_Insert.Add("SERVICIO_INTERES", ServiciosInteres);
                Arr_Insert.Add("FECHA_CREACION", FechaCreacion.ToString("dd/MM/yyyy"));
                Arr_Insert.Add("USUARIO_ASIGNADO", UsuarioAsignado);
                Arr_Insert.Add("ESTADO", "1");
                Arr_Insert.Add("ID_COMERCIO", IDComercio);
                bool respuesta = MYSQL_C.MYSQLInsert("BILLINGMYSQL_1", "PUNTO_AFILIACION_TICKET", Arr_Insert, "InsertaLead", "");

                if (respuesta)
                {
                    return Json("OK");
                }
                else
                {
                    return Json("ERROR");
                }
            }
            catch (Exception e)
            {

                return View("Login/Login");
            }

        }

        public ActionResult PaquetesCombos()
        {
            string JSON = string.Empty;

            DataTable Paquete = MYSQL_C.MYSQLSelect("BILLINGMYSQL_1", "SELECT DESCRIPCIO FROM PAQUETES_ENC WHERE punto_afiliacion = 1");
            JSON = JsonConvert.SerializeObject(Paquete);


            return Json(JSON);
        }

    }
}