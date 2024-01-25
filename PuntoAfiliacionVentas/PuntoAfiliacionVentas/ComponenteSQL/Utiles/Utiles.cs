using PuntoAfiliacionVentas.ComponenteSQL.Conexion;
using PuntoAfiliacionVentas.ComponenteSQL.Trydicionary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PuntoAfiliacionVentas.ComponenteSQL.Utiles
{
    class Utiles
    {

        //Creado por: Telecable(BJZ)
        //Nombre: SUBSTR
        //Descripción: Devuelve el valor recortado
        //Fecha de creación: 09/07/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public String SUBSTR(string cadena, int inicio, int tamano)
        {
            if (cadena.Length > tamano)//Si el valor que se le envia es mayor al tamaño maximo lo recorta
            {
                return cadena.Substring(inicio, tamano);
            }
            else//Sino puede recortarlo, inicia la cadena donde se le indico y termina hasta el maximo de caracteres que tenga
            {
                if (cadena.Length > 0)
                {
                    return cadena.Substring(inicio, (cadena.Length - inicio));
                }
                else
                {
                    return cadena;
                }

            }
        }

        //Creado por: Telecable(BJZ)
        //Nombre: Migracion_Data_MYSQL
        //Descripción: Se encarga de migrar data de adv a mysql para comenzar con la unificación
        //Fecha de creación: 01/09/2021
        //Fecha Modificación:
        //Cambios Realizados:

        //Parametros 
        //Arr_INS_UPD= Array Insert/Update dependiendo del tipo de ejecución <"columna","valor">
        //Arr_Where= Triccionario con el Where <"columna","valor","operador">
        //tipo_exec= Tipo de ejecución INSERT/UPDATE/DELETE
        //tabla= Tabla a afectar en MySQL
        public Boolean Migracion_Data_MYSQL(Dictionary<string, object> Arr_INS_UPD, Trictionary<string, object, string[]> Arr_Where, string tipo_exec, string tabla, string metodo, string contrato)
        {


            //****************HEREDA LAS CLASES DE MYSQL**********************
            PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL mysql = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();
            //***********************FIN HEREDA*******************************


            /****************************VARIABLES*********************************/

            bool respuesta_exec = false;

            /*******************************FIN************************************/

            //Consultamos si la tabla ya existe en MySQL y está lista para la migración
            System.Data.DataTable Tabla_Consultar = new System.Data.DataTable();

            //Tabla Nueva de migración, con columna de activo
            //Tabla_Consultar = mysql.MYSQLSelect("BILLINGMYSQL", "SELECT nombre_tabla activo FROM" +
            //    " migracion_tablas_adv WHERE nombre_tabla='" + tabla + "' AND activo=1");

            //Tabla Original de migración por si se debe hacer el switch Johnny
            Tabla_Consultar = mysql.MYSQLSelect("BILLINGMYSQL", "SELECT * FROM tabla_migrada WHERE lower(data_migra)=lower('" + tabla + "')");

            try
            {
                if (Tabla_Consultar.Rows.Count > 0)//Si existe y está activo, procedemos a realizar la debida ejecución unicamente a la tabla Billing de MYSQL
                {
                    if (Tabla_Consultar.Rows[0]["data_migra"].ToString().Trim() == tabla.ToLower().Trim())//Consulta que la tabla migrada sea igual
                    {
                        if ((tipo_exec.ToLower() == "insert"))//Si es de tipo insert
                        {
                            //Almacenamos la respuesta de nuestra sentencia
                            respuesta_exec = mysql.MYSQLInsert("BILLINGMYSQL", tabla, Arr_INS_UPD, metodo, contrato);

                        }
                        else if ((tipo_exec.ToLower() == "update"))//Si es de tipo update
                        {
                            //Consultar con Johnny si los UPDATES y DELETES deben viajar tambien

                            //Almacenamos la respuesta de nuestra sentencia
                            respuesta_exec = mysql.MYSQLUpdate("BILLINGMYSQL", tabla, Arr_INS_UPD, Arr_Where, metodo, contrato);
                        }
                        else if (tipo_exec.ToLower() == "delete")//Si es de tipo delete
                        {
                            //Consultar con Johnny si los UPDATES y DELETES deben viajar tambien

                            //Almacenamos la respuesta de nuestra sentencia
                            respuesta_exec = mysql.MYSQLDelete("BILLINGMYSQL", tabla, Arr_Where, metodo, contrato);
                        }
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }



            return respuesta_exec;
        }


        //Creado por: Telecable(BJZ)
        //Nombre: Migracion_Data_MYSQL_Directo
        //Descripción: Se encarga de migrar data de adv a mysql para comenzar con la unificación
        //Fecha de creación: 01/09/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public Boolean Migracion_Data_MYSQL_Directo(string query, string tabla, string metodo, string contrato)
        {


            //****************HEREDA LAS CLASES DE MYSQL**********************
            PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL mysql = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();
            //***********************FIN HEREDA*******************************


            /****************************VARIABLES*********************************/

            bool respuesta_exec = false;

            /*******************************FIN************************************/

            //Consultamos si la tabla ya existe en MySQL y está lista para la migración
            System.Data.DataTable Tabla_Consultar = new System.Data.DataTable();

            //Tabla Nueva de migración, con columna de activo
            //Tabla_Consultar = mysql.MYSQLSelect("BILLINGMYSQL", "SELECT nombre_tabla activo FROM" +
            //    " migracion_tablas_adv WHERE nombre_tabla='" + tabla + "' AND activo=1");

            //Tabla Original de migración por si se debe hacer el switch Johnny
            Tabla_Consultar = mysql.MYSQLSelect("BILLINGMYSQL", "SELECT * FROM tabla_migrada WHERE lower(data_migra)=lower('" + tabla + "')");

            if (Tabla_Consultar.Rows.Count > 0)//Si existe y está activo, procedemos a realizar la debida ejecución unicamente a la tabla Billing de MYSQL
            {
                if (Tabla_Consultar.Rows[0]["data_migra"].ToString().Trim() == tabla.ToLower().Trim())//Consulta que la tabla migrada sea igual
                {
                    respuesta_exec = mysql.MYSQLDirecto("BILLINGMYSQL", tabla, query, metodo, contrato);
                }
            }

            return respuesta_exec;
        }


        //Creado por: Telecable(BJZ)
        //Nombre: Bitacora_Agencias
        //Descripción: Inserta un registro de los insert o updates que se hagan
        //Fecha de creación: 14/07/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public void Bitacora_ComponenteSQL(string usuario, string query, bool estado, string bd, string metodo, string contrato)
        {

            PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL mysql = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();

            Dictionary<string, object> Arr_Insert = new Dictionary<string, object>();// Diccionario para insertar, modificar valores

            string tipo_exec = "";

            int estadoi = 1;

            if (!String.IsNullOrEmpty(query))//Mientras no venga vacio
            {
                query = query.Replace("\r\n", " ");//Reemplazamos saltos de linea por espacios
                query = query.Replace("'", "´");// Remplazamos la coma por una que no de problemas

                if (query.ToLower().Contains("insert"))//Evalua de que tipo es
                {
                    query = query.Replace("INSERT", "");
                    query = query.Replace("insert", "");

                    tipo_exec = "INSERT";
                }
                else if (query.ToLower().Contains("update"))//Evalua de que tipo es
                {
                    query = query.Replace("UPDATE", "");
                    query = query.Replace("update", "");

                    tipo_exec = "UPDATE";
                }
                else if (query.ToLower().Contains("delete"))//Evalua de que tipo es
                {
                    query = query.Replace("DELETE", "");
                    query = query.Replace("delete", "");

                    tipo_exec = "DELETE";
                }
            }

            if (estado == false)
            {
                Envia_Correo("Componente SQL: Se ha generado un error al realizar la sentencia: <font style='font-weight:bold;color:red'>" +
                tipo_exec + " " + query + "</font> <p>De la Base de Datos: " + bd + "</p><p>Sistema de Origen: " + "OTT_WCF/" + metodo + "</p> <p>Por el usuario: " + usuario + "</p>", "Componente SQL: Error al ejecutar query");
                estadoi = 0;
            }


            Arr_Insert.Clear();
            Arr_Insert.Add("tipo", tipo_exec);
            Arr_Insert.Add("cuerpo", query);
            Arr_Insert.Add("bd", bd);
            Arr_Insert.Add("estado", estadoi);
            Arr_Insert.Add("fecha", DateTime.Now.ToString("dd/MM/yyyy"));
            Arr_Insert.Add("hora", DateTime.Now.ToString("HH:mm:ss"));
            Arr_Insert.Add("usuario", usuario);
            Arr_Insert.Add("origen", "OTT_WCF/" + metodo);//Sistema de Origen
            Arr_Insert.Add("contrato", contrato);//Sistema de Origen


            //SE COMENTA
            mysql.MYSQLInsert("BILLINGMYSQL", "bitacora_componente_sql", Arr_Insert, "", "");//Almacenamos la respuesta del query  No es necesario ni metodo ni contrato

        }


        //Creado por: Telecable(BJZ)
        //Nombre: Bitacora_Agencias
        //Descripción: Inserta un registro de los insert o updates que se hagan
        //Fecha de creación: 14/07/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public void Bitacora_ComponenteSQL_EXECEP(string datos, string tipo, string tabla, string usuario, string bd, string where, string metodo, string contrato, string error)
        {
            PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL mysql = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();

            Dictionary<string, object> Arr_Insert = new Dictionary<string, object>();// Diccionario para insertar, modificar valores

            string final_query;

            if (IsNullOrEmpty(datos))
            {
                datos = "";
            }
            else
            {
                datos = "CUERPO: " + datos + " ";
            }

            if (tipo == "UPDATE" || tipo == "DELETE")
            {
                datos = datos + "WHERE: " + where;
            }

            datos = datos.Replace("'", "´");// Limpiamos los datos

            Envia_Correo("Componente SQL: Se ha generado un error al realizar la sentencia (EXEPCION O Catch): <font style='font-weight:bold;color:red'>" +
            tipo + " " + datos + "</font> <p>De la Base de Datos: " + bd + "</p><p>De la tabla: " + tabla + "</p><p>Sistema de Origen: " + "OTT_WCF/" + metodo + "</p> <p>Por el usuario: "
            + usuario + "</p> <p>Catch Error: " + error + "</p>", "Componente SQL(EXEPCION O Catch): Error al ejecutar query");


            //Arr_Insert.Clear();
            //Arr_Insert.Add("tipo", tipo);
            //Arr_Insert.Add("cuerpo", datos);
            //Arr_Insert.Add("bd", bd);
            //Arr_Insert.Add("estado", 0);
            //Arr_Insert.Add("fecha", DateTime.Now.ToString("dd/MM/yyyy"));
            //Arr_Insert.Add("hora", DateTime.Now.ToString("HH:mm:ss"));
            //Arr_Insert.Add("usuario", usuario);
            //Arr_Insert.Add("origen", "AGENCIAS/" + metodo);//Sistema de Origen
            //Arr_Insert.Add("contrato", contrato);//Sistema de Origen

            //mysql.MYSQLInsert("BILLINGMYSQL", "bitacora_componente_sql", Arr_Insert, "", "");//Almacenamos la respuesta del query  No es necesario ni metodo ni contrato

        }


        public string GetLine(Dictionary<string, object> data)
        {
            // Build up the string data.
            StringBuilder builder = new StringBuilder();
            foreach (var pair in data)
            {
                builder.Append('´').Append(pair.Key).Append('´').Append("=").Append('´').Append(pair.Value).Append('´').Append(',');
            }
            string result = builder.ToString();
            // Remove the end comma.
            result = result.TrimEnd(',');
            return result;
        }

        public string GetLine_Triccionary(Trictionary<string, object, string[]> data)//Arma una cadena String a base de un triccionario donde se almacena un where
        {
            // Build up the string data.
            StringBuilder builder = new StringBuilder();
            foreach (var pair in data)
            {
                if (IsNullOrEmpty(pair.Control[0]))
                {
                    pair.Control[0] = "=";
                }

                if (IsNullOrEmpty(pair.Control[1]))
                {
                    pair.Control[1] = "AND";
                }

                builder.Append('{').Append(pair.Key).Append(pair.Control[0]).Append(pair.Value).Append(" Operadores: [").Append(pair.Control[0]).Append(';').Append(pair.Control[1]).Append(']').Append("},");
            }
            string result = builder.ToString();
            // Remove the end comma.
            result = result.TrimEnd(',');
            return result;
        }


        public bool Envia_Correo(string cuerpo, string asunto)
        {

            try
            {
                PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL mysql = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.MYSQL();
                string password = string.Empty;
                DataTable parametros = mysql.MYSQLSelect("BILLINGMYSQL", "SELECT * FROM parametros_web WHERE codigo = '24'");
                string correo = string.Empty;
                if (parametros.Rows.Count > 0)
                {
                    correo = parametros.Rows[0]["descripcion"].ToString();
                    password = parametros.Rows[0]["valor"].ToString();
                }
                var basicCredential = new NetworkCredential(correo, password); //"Telecable01");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");
                SmtpServer.Credentials = basicCredential;
                mail.From = new MailAddress(correo, "Telecable S.A.");
                MailAddress bcc = new MailAddress("bruce.jimenez@telecablecr.com");
                MailAddress bcc2 = new MailAddress("orlin.castillo@telecablecr.com");
                MailAddress bcc3 = new MailAddress("julio.hernandez@telecablecr.com");

                mail.Bcc.Add(bcc);
                mail.Bcc.Add(bcc2);
                mail.Bcc.Add(bcc3);

                string data = string.Empty;

                mail.Subject = asunto;

                mail.Body = cuerpo;
                mail.IsBodyHtml = true;


                SmtpServer.Port = 587;
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);


                return true; // En caso de enviar el correo exitosamente

            }
            catch (Exception ex)
            {
                string error = "Error: " + ex.Message.ToString();
                return false;// En caso de no enviar el correo

            }
        }

        //Creado por: Telecable(BJZ)
        //Nombre: GetTypeVar
        //Descripción: Devuelve el tipo de dato
        //Fecha de creación: 30/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public String GetTypeVar(string variable)
        {
            string vartype = "C";//Variable a retornar, se setea que devuelva String por default

            /*Variables booleanas*/
            bool booleano;
            /*Fin variables booleanas*/

            /*Variables númericas*/
            double number_d;//Guarda temporal
            int number_i;//Guarda temporal
            short number_i_2;

            /*Fin variables*/

            if (!string.IsNullOrEmpty(variable))
            {
                variable = EliminarCaracteresEspeciales(variable);//Si caracteres especiales, se los quita
            }

            /*Variables de fechas*/
            DateTime fecha;
            /*Fin fechas*/

            /*
			** C = Character, Memo, Varchar, Varchar (Binary)
			** L = Logical
			** N = Numeric, Float, Double, Integer
			** D = DateTime
			*/

            /*Booleano*/
            if (Boolean.TryParse(variable, out booleano))
            {
                vartype = "L";

                if (variable == "true" || variable == "TRUE" || variable == "True" || variable == "1")
                {
                    variable = "1";
                }
                else
                {
                    variable = "0"; // valor en falso

                }
            }
            /*Fin Booleano*/

            /*Numericos*/
            if (Int16.TryParse(variable, out number_i_2))
            {
                vartype = "N";

                if (variable == "true" || variable == "TRUE" || variable == "True" || variable == "1")
                {
                    variable = "1";
                }
                else if (variable == "false" || variable == "FALSE" || variable == "False" || variable == "0" || String.IsNullOrEmpty(variable))
                {
                    variable = "0"; // valor en falso

                }

            }

            if (Int32.TryParse(variable, out number_i))
            {
                vartype = "N";
            }

            if (Double.TryParse(variable, out number_d))
            {
                vartype = "N";
            }

            /*Fin númericos*/

            /*Fechas, devuelve fecha solo para MYSQL*/
            if (DateTime.TryParseExact(variable, "dd/MM/yyyy", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out fecha))
            {
                vartype = "D";

                if (fecha.ToString("yyyy/MM/dd") == "1899/12/30" || fecha.ToString("yyyy/MM/dd") == "1899/12/31")
                {
                    variable = "'1900/01/01 00:00:00'";
                }
                else
                {
                    variable = "'" + fecha.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                }


            }
            else
            {

                variable = "'1900/01/01 00:00:00'";

            }
            /*Fin fechas*/

            /*Si es caracter, entonces concatenele comillas*/
            if (vartype == "C")
            {
                variable = "'" + variable + "'";
            }

            return variable;
        }

        public string EliminarCaracteresEspeciales(string valor)
        {
            if (!String.IsNullOrEmpty(valor))
            {
                valor = valor.Trim();
                valor = valor.Replace("#", " ");
                valor = valor.Replace("<", " ");
                valor = valor.Replace("(", " ");
                valor = valor.Replace("[", " ");
                valor = valor.Replace("'", " ");
                valor = valor.Replace("¡", " ");
                valor = valor.Replace("=", " ");
                valor = valor.Replace("¦", " ");
                valor = valor.Replace("©", " ");
                valor = valor.Replace("º", " ");
                valor = valor.Replace("¯", " ");
                valor = valor.Replace("¹", " ");
                valor = valor.Replace("³", " ");
                valor = valor.Replace("µ", " ");
                valor = valor.Replace("±", " ");
                valor = valor.Replace("¼", " ");
                valor = valor.Replace("¾", " ");
                //*valor = valor.Replace( "?", " ");
                valor = valor.Replace("ç", " ");
                valor = valor.Replace("Ñ", "N");
                //*valor = valor.Replace( "ñ", 'n');
                valor = valor.Replace("×", " ");
                //*valor = valor.Replace( "-", " ");
                valor = valor.Replace(@"\", " ");
                //* valor = valor.Replace( "/", " ");
                valor = valor.Replace("ø", " ");
                //*valor = valor.Replace( ":", '');
                valor = valor.Replace("…", " ");
                valor = valor.Replace("&", " ");
                valor = valor.Replace("$", " ");
                valor = valor.Replace(">", " ");
                valor = valor.Replace("™", " ");
                valor = valor.Replace(")", " ");
                valor = valor.Replace("]", " ");
                valor = valor.Replace("€", " ");
                valor = valor.Replace("!", " ");
                valor = valor.Replace("¤", " ");
                valor = valor.Replace("¨", " ");
                valor = valor.Replace("ª", " ");
                valor = valor.Replace("®", " ");
                valor = valor.Replace("°", " ");
                valor = valor.Replace("²", " ");
                valor = valor.Replace("´", " ");
                valor = valor.Replace("¸", " ");
                valor = valor.Replace('"', ' ');
                valor = valor.Replace("½", " ");
                valor = valor.Replace("¿", " ");
                valor = valor.Replace(",", " ");
                valor = valor.Replace("÷", " ");
                valor = valor.Replace("+", " ");
                valor = valor.Replace("*", " ");
                valor = valor.Replace(";", " ");
                //*valor = valor.Replace( "@", '');
                valor = valor.Replace("Á", "A");
                valor = valor.Replace("É", "E");
                valor = valor.Replace("Í", "I");
                valor = valor.Replace("Ó", "O");
                valor = valor.Replace("Ú", "U");
                valor = valor.Replace("á", "a");
                valor = valor.Replace("é", "e");
                valor = valor.Replace("í", "i");
                valor = valor.Replace("ó", "o");
                valor = valor.Replace("ú", "u");

                /*Elimina saltos de linea*/
                valor = valor.Replace("\r\n", " ");
                valor = valor.Replace("\n", " ");
                valor = valor.Replace("\r", " ");
            }
            else
            {
                valor = " ";
            }


            return valor;

        }


        //public void CrearExcel_DT(System.Data.DataTable Exportar, string tabla)
        //{
        //    /*Convertimos el DataTable en Excel*/
        //    XLWorkbook ArchivoExcel = new XLWorkbook();
        //    ArchivoExcel.Worksheets.Add(Exportar, tabla);

        //    string ruta = System.IO.Directory.GetCurrentDirectory();

        //    /*Y lo guardamos*/
        //    ArchivoExcel.SaveAs(ruta + "Reporte_InsercionMYSQL_Masivo_" + tabla + "_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx");

        //}


        public bool DateTimeIsNullOrEmpty(string Date)
        {
            // Si viene nula,                Si es formato ADV,       Si es formato MYSQL,        Si es formato C#
            if (IsNullOrEmpty(Date) || Date.Contains("1899") || Date.Contains("1900") || Date.Contains("0001"))
            {//Está vacía
                return true;
            }
            else
            {//No está vacía
                return false;
            }
        }

        public bool TrueFalseBool(object Booleano)//Anteriormente llamado Val_Bool
        {
            String Booleano_S;
            if (!IsNullOrEmpty(Booleano))//Mientras sea diferente de vacio
            {
                Booleano_S = Booleano.ToString().Trim();
                Booleano_S = Booleano.ToString().ToLower();
                if (Booleano_S == "1" || Booleano_S == "true")//Si fuera true
                {
                    return true;
                }
                else//Si no es false
                {
                    return false;
                }
            }
            else//Si no se tomara como un false
            {
                return false;
            }
        }


        public bool IsNullOrEmpty(object Objeto)
        {
            if (Objeto == null)
            {
                return true;
            }
            else
            {
                if (Objeto.ToString() == "")
                {
                    return true;
                }
            }

            return false;

        }

        public string LimpiaFecha(object Fecha)
        {
            if (!IsNullOrEmpty(Fecha))
            {
                string Fecha_Text = Fecha.ToString();

                Fecha_Text = Fecha_Text.Replace("a.m.", "AM");
                Fecha_Text = Fecha_Text.Replace("a.m. ", "AM");
                Fecha_Text = Fecha_Text.Replace("a. m.", "AM");
                Fecha_Text = Fecha_Text.Replace("a. m. ", "AM");

                Fecha_Text = Fecha_Text.Replace("p.m.", "PM");        
                Fecha_Text = Fecha_Text.Replace("p.m. ", "PM");
                Fecha_Text = Fecha_Text.Replace("p. m.", "PM");
                Fecha_Text = Fecha_Text.Replace("p. m. ", "PM");

                return Fecha_Text;

            }
            else
            {
                return "";//Si no viene devuelve vacio
            }
        }

        public string LimpiarFechaADV(object Fecha)
        {
            if (!IsNullOrEmpty(Fecha))
            {
                string Fecha_ = SUBSTR(Fecha.ToString().Trim(), 0, 10);//Recorta la fecha
                DateTime FECHA_FORMAT;
                string[] formats = { "MM/dd/yyyy", "MM-dd-yyyy" };




                if (DateTime.TryParseExact(Fecha_, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
                {
                    return FECHA_FORMAT.ToString("dd/MM/yyyy");
                }
                else//Si no logra convertir
                {
                    return "";
                }
            }
            else//Si viene vacia
            {
                return "";
            }



        }
        public string LimpiarFechaMYSQL(object Fecha)
        {
            if (!IsNullOrEmpty(Fecha))
            {
                string Fecha_ = SUBSTR(Fecha.ToString().Trim(), 0, 10);//Recorta la fecha
                DateTime FECHA_FORMAT;
                string[] formats = { "yyyy/MM/dd", "yyyy-MM-dd", "yyyy-M-d", "yyyy/M/d", "yyyy-MM-d", "yyyy/MM/d", "yyyy-M-dd", "yyyy/M/dd" };

                if (DateTime.TryParseExact(Fecha_, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
                {
                    return FECHA_FORMAT.ToString("dd/MM/yyyy");
                }
                else//Si no logra convertir
                {
                    return "";
                }
            }
            else//Si viene vacia
            {
                return "";
            }
        }


    }
}