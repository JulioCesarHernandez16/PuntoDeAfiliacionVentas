using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PuntoAfiliacionVentas.ComponenteSQL.Trydicionary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuntoAfiliacionVentas.Controllers.Casos
{
    public class CasosController : Controller
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
        // GET: Casos
        public ActionResult Casos()
        {
            return View();
        }

        public string CargaCasosLead()
        {
            string ID_COMERCIO = "1";
            string select = "SELECT * FROM PUNTO_AFILIACION_TICKET WHERE ID_COMERCIO = "+ ID_COMERCIO + "";
            DataTable Lead = MYSQL_C.MYSQLSelect("BILLINGMYSQL_1", select);
            string Json = JsonConvert.SerializeObject(Lead);
            return Json;
        }
    }
}