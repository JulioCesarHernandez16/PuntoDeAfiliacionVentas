using PuntoAfiliacionVentas.ComponenteSQL.Trydicionary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using PuntoAfiliacionVentas.ComponenteSQL.Utiles;
using System.Diagnostics;
using System.Web;

namespace PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL
{
    public class MYSQL
    {
        ///_____________________________________________BUENAS PRACTICAS___________________________________________________________________________________
        /// - Utilizar siempre formato para fecha que sea dd/MM/yyyy o dd-MM-yyyy(o sus variantes, pero que sea en ese orden)
        /// - Para los números con decimales es importante saber que su separador de decimal siempre debe ser "." Si se envía "," automaticamente
        /// lo detectara como separador de miles. Casos del mundo ideal: 
        /// A)1000.50  
        /// B)1,000.50
        /// Funciona pero intentar no utilizar:
        /// C)1.000,50
        /// Nunca utilizar
        /// D)1,000(Lo interpretara como miles)
        /// -Todo UPDATE o DELETE debe llevar sus respectivos parametros de WHERE, sino el query se va a caer
        /// -Todo UPDATE o DELETE si es booleano,númerico o de fecha no debe ir vacío, sino se va a caer la consulta. Esto para mantener las integridad de los datos,
        /// todo where debe ser exacto

        PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.Conexion_Mysql conexion = new PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL.Conexion_Mysql();
        CultureInfo newCulture = CultureInfo.CreateSpecificCulture("en-US");
        //Unicamente utilizar Cultura en Estados Unidos

        HttpContext context = HttpContext.Current;

        /*Carga Utiles*/
        PuntoAfiliacionVentas.ComponenteSQL.Utiles.Utiles Util = new PuntoAfiliacionVentas.ComponenteSQL.Utiles.Utiles();
        numerales callNumerales = new numerales();
        fecha callFecha = new fecha();
        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de procesar las consultas a MSYQL
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public DataTable MYSQLSelect(string tipoconexion, string query, string maxlength = "0")
        {

            //newCulture = new CultureInfo("es-CR");
            //CultureInfo.DefaultThreadCurrentCulture = newCulture;
            //CultureInfo myCIclone = (CultureInfo)newCulture.Clone();
            //myCIclone.NumberFormat.NumberDecimalDigits = 2;
            //myCIclone.NumberFormat.NumberDecimalSeparator = ".";

            DataTable tmpMySqlSelectCur = new DataTable(); //variable que almacena la informacion del query a consultar

            string mySqlSelectconex = "";
            int johexitcant = 0;
            int johexitbandera = 0;
            try
            {
                //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
                while (johexitbandera < 1 && johexitcant < 10)
                {
                    mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
                    if (!string.IsNullOrEmpty(mySqlSelectconex))
                    {
                        if (!mySqlSelectconex.Contains("error"))
                        {
                            johexitbandera = 1;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                        //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
                    }
                    else
                    {
                        Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
                    }
                    johexitcant = johexitcant + 1;
                }

                if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex))
                {

                    if (!string.IsNullOrEmpty(query))
                    {
                        try
                        {

                            Console.WriteLine(CultureInfo.DefaultThreadCurrentCulture);
                            //ME TRAE LA CULTURA DEL SERVIDOR ACTUAL

                            tmpMySqlSelectCur = conexion.select_Mysql(query, mySqlSelectconex);

                            //Establecemos la conexión y consultamos el query que trajimos de parametros

                            if (tmpMySqlSelectCur != null)
                            {

                                if (tmpMySqlSelectCur.Columns.Count > 0)
                                {
                                    //Esto nos indica si la columna que recibio es la una excepcion(Error)

                                    if (tmpMySqlSelectCur.Columns[0].ColumnName.Contains("Exception_mysql_")) //Si la columna tiene este encabezado entonces
                                    {
                                        return tmpMySqlSelectCur; //Retornamos el error
                                    }


                                }

                                /*VERIFICAMOS EL TIPO DE CAMPOS DE LA TABLA*/
                                DataColumnCollection columns = tmpMySqlSelectCur.Columns;
                                //for (int j = 0; j < tmpMySqlSelectCur.Rows.Count; j++)
                                //{

                                for (int i = 0; i < columns.Count; i++)
                                {

                                    string NombreColumna = columns[i].ColumnName.ToString();//Almacenamos el nombre de la columna
                                    string TipoDatoColumna = columns[i].DataType.Name.ToUpper().ToString();//Almacenamos el tipo de la columna

                                    tmpMySqlSelectCur.Columns[NombreColumna].ReadOnly = false;//Elimina el readonly de la columna para evitar problemas
                                    tmpMySqlSelectCur.Columns[NombreColumna].AllowDBNull = true;//Permite nulos en las tablas para evitar conflictos

                                    if (maxlength == "0")
                                    {
                                        tmpMySqlSelectCur.Columns[NombreColumna].MaxLength = -1;
                                    }

                                    for (int j = 0; j < tmpMySqlSelectCur.Rows.Count; j++)
                                    {
                                        DataRow info = tmpMySqlSelectCur.Rows[j];
                                        string Valor_fila = info[NombreColumna].ToString().Trim();

                                        switch (TipoDatoColumna)
                                        {
                                            case "STRING": // si es STRING ENTRA A ESTE CASO 
                                            case "BYTE[]": // si es STRING ENTRA A ESTE CASO 
                                                if (string.IsNullOrEmpty(Valor_fila) || Valor_fila == "System.BYTE[]")
                                                {

                                                    info.BeginEdit();

                                                    info[NombreColumna] = "";

                                                    info.AcceptChanges();
                                                }


                                                break;

                                            case "INT16"://Usualmente usado como booleano
                                                         //Recordar que el valor devuelto por estos, siempre será un 0(False) o un 1(True)
                                                         //Pero siempre será númerico. El desarrollador debe transformarlo a conveniencia

                                                if (string.IsNullOrEmpty(Valor_fila))
                                                {
                                                    info.BeginEdit();

                                                    info[NombreColumna] = 0;

                                                    info.AcceptChanges();
                                                }

                                                break;


                                            case "DOUBLE":
                                            case "INT32":
                                            case "INT64":
                                            case "DECIMAL":
                                            case "FLOAT":
                                                //Si es númerico y viene vacío se convierte en 0
                                                if (string.IsNullOrEmpty(Valor_fila))
                                                {
                                                    info.BeginEdit();

                                                    info[NombreColumna] = 0;

                                                    info.AcceptChanges();
                                                }

                                                break;


                                            case "DATETIME":
                                                DateTime FECHA_FORMAT;


                                                /*
                                                 Formatos Aceptados
                                                 Dia/Mes/Año Hora:minutos:segundos-------------- Escenario Ideal
                                                 Dia/Mes/Año -------------- Escenario Ideal
                                                 Año/Mes/dia 

                                                 */

                                                /*Validacion de tt, C# no admite el formato con puntos! Limpiamos la fecha si viene así*/
                                                //var Date = "";//Almacena la fecha

                                                //if (Valor_fila.Contains("a.m.") || Valor_fila.Contains("a. m."))
                                                //{
                                                //    Date = Valor_fila.Replace("a.m.", "AM");
                                                //    Date = Valor_fila.Replace("a. m.", "AM");
                                                //}
                                                //else if (Valor_fila.Contains("p.m.") || Valor_fila.Contains("p. m."))
                                                //{
                                                //    Date = Valor_fila.Replace("p.m.", "PM");
                                                //    Date = Valor_fila.Replace("p. m.", "PM");
                                                //}
                                                //else//Si no contiene ninguno
                                                //{
                                                //    Date = Valor_fila;
                                                //}

                                                string Date = Valor_fila;//Almacena la fecha

                                                Date = Util.LimpiaFecha(Date);//Limpiamos la fecha

                                                /*Fin validacion*/

                                                string[] formats = { 
                                                 //DIA/MES/ANNO
                                                 "dd/M/yyyy", "dd/M/yyyy HH:mm:ss", "d/M/yyyy", "d/M/yyyy h:mm:ss", "d/M/yyyy h:mm:ss tt", "dd/MM/yyyy hh:mm:ss",
                                                 "d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt","d/M/yyyy hh:mm tt","d/M/yyyy hh tt",
                                                 "d/M/yyyy h:mm","d/M/yyyy h:mm","dd/MM/yyyy hh:mm", "dd/M/yyyy hh:mm",
                                                 //Nuevo
                                                 "dd/MM/yyyy hh:mm:ss tt",
                                                 "d/M/yyyy HH:mm:ss",
                                                 "dd/MM/yyyy HH:mm:ss",
                                                 //DIA-MES-ANNO
                                                 "dd-M-yyyy", "dd-M-yyyy HH:mm:ss", "d-M-yyyy", "d-M-yyyy h:mm:ss", "d-M-yyyy h:mm:ss tt", "dd-MM-yyyy hh:mm:ss",
                                                 "d-M-yyyy h:mm:ss tt", "d-M-yyyy h:mm tt","d-M-yyyy hh:mm tt","d-M-yyyy hh tt",
                                                 "d-M-yyyy h:mm","d-M-yyyy h:mm","dd-MM-yyyy hh:mm", "dd-M-yyyy hh:mm",
                                                 //Nuevo
                                                 "dd-MM-yyyy hh:mm:ss tt",
                                                 "d-M-yyyy HH:mm:ss"


                                                  };
                                                if (!DateTime.TryParseExact(Date, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
                                                {//NO EXISTE UN FORMATO VALIDO 


                                                    if (Util.IsNullOrEmpty(Date))//Si la fecha es vacia
                                                    {
                                                        info.BeginEdit();

                                                        info[NombreColumna] = Convert.ToDateTime("1900/01/01");//Una columna fecha no debe ir vacia, se rellena

                                                        info.AcceptChanges();
                                                    }


                                                }
                                                else
                                                {

                                                }



                                                break;
                                        }




                                    }


                                }


                                //}



                                return tmpMySqlSelectCur;
                                //SI NO TRAE EXCEPCION ENTONCES VAMOS A RECORRER LA TABLA ENTRANTE Y VERIFICAR TIPOS DE DATOS



                            }
                            else
                            {
                                //mySqlSelectconex = "TABLA NULA";
                                //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                                //tmpMySqlSelectCur.Rows.Add("ERROR " + mySqlSelectconex + "  QUERY - " + query);
                                return tmpMySqlSelectCur;
                            }
                        }
                        catch (Exception error)
                        {

                            //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                            //tmpMySqlSelectCur.Rows.Add("ERROR " + mySqlSelectconex + "  QUERY - " + query);
                            return tmpMySqlSelectCur;
                        }



                    }
                }
                else
                {
                    //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                    //tmpMySqlSelectCur.Rows.Add("ERROR " + mySqlSelectconex + "  QUERY - " + query);
                    return tmpMySqlSelectCur;
                }
            }
            catch (Exception e)
            {
                //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                //tmpMySqlSelectCur.Rows.Add("ERROR " + e.ToString());
                return tmpMySqlSelectCur;
            }





            return tmpMySqlSelectCur;
        }

        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo encargado de procesar las consultas a MSYQL
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public DataTable MYSQLSelect_Masivo(string tipoconexion, string query, string maxlength = "0")
        {

            //newCulture = new CultureInfo("es-CR");
            //CultureInfo.DefaultThreadCurrentCulture = newCulture;
            //CultureInfo myCIclone = (CultureInfo)newCulture.Clone();
            //myCIclone.NumberFormat.NumberDecimalDigits = 2;
            //myCIclone.NumberFormat.NumberDecimalSeparator = ".";

            DataTable tmpMySqlSelectCur = new DataTable(); //variable que almacena la informacion del query a consultar

            string mySqlSelectconex = "";
            int johexitcant = 0;
            int johexitbandera = 0;
            try
            {
                //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
                while (johexitbandera < 1 && johexitcant < 10)
                {
                    mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
                    if (!string.IsNullOrEmpty(mySqlSelectconex))
                    {
                        if (!mySqlSelectconex.Contains("error"))
                        {
                            johexitbandera = 1;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                        //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
                    }
                    else
                    {
                        Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
                    }
                    johexitcant = johexitcant + 1;
                }

                if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex))
                {

                    if (!string.IsNullOrEmpty(query))
                    {
                        try
                        {

                            Console.WriteLine(CultureInfo.DefaultThreadCurrentCulture);
                            //ME TRAE LA CULTURA DEL SERVIDOR ACTUAL

                            tmpMySqlSelectCur = conexion.select_Mysql(query, mySqlSelectconex);

                            DataColumnCollection columns = tmpMySqlSelectCur.Columns;

                            for (int i = 0; i < columns.Count; i++)
                            {

                                string NombreColumna = columns[i].ColumnName.ToString();//Almacenamos el nombre de la columna
                                string TipoDatoColumna = columns[i].DataType.Name.ToUpper().ToString();//Almacenamos el tipo de la columna

                                tmpMySqlSelectCur.Columns[NombreColumna].ReadOnly = false;//Elimina el readonly de la columna para evitar problemas
                                tmpMySqlSelectCur.Columns[NombreColumna].AllowDBNull = true;//Permite nulos en las tablas para evitar conflictos

                                if (maxlength == "0")
                                {
                                    tmpMySqlSelectCur.Columns[NombreColumna].MaxLength = -1;
                                }

                            }

                            //Establecemos la conexión y consultamos el query que trajimos de parametros

                            if (tmpMySqlSelectCur == null)
                            {
                                //mySqlSelectconex = "TABLA NULA";
                                //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                                //tmpMySqlSelectCur.Rows.Add("ERROR " + mySqlSelectconex + "  QUERY - " + query);
                                return tmpMySqlSelectCur;
                            }
                        }
                        catch (Exception error)
                        {

                            //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                            //tmpMySqlSelectCur.Rows.Add("ERROR " + mySqlSelectconex + "  QUERY - " + query);
                            return tmpMySqlSelectCur;
                        }



                    }
                }
                else
                {
                    //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                    //tmpMySqlSelectCur.Rows.Add("ERROR " + mySqlSelectconex + "  QUERY - " + query);
                    return tmpMySqlSelectCur;
                }
            }
            catch (Exception e)
            {
                //tmpMySqlSelectCur.Columns.Add("Exception_mysql_");
                //tmpMySqlSelectCur.Rows.Add("ERROR " + e.ToString());
                return tmpMySqlSelectCur;
            }





            return tmpMySqlSelectCur;
        }


        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo de insercción a MSYQL
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        public bool MYSQLInsert(string tipoconexion, string tablaInsert, Dictionary<string, object> Dict_Insert, string metodo, string contrato)
        {
            /*Usuario*/
            string Usuario = "";
            /*Fin Usuario*/

            //newCulture = new CultureInfo("en-US");
            //CultureInfo.DefaultThreadCurrentCulture = newCulture;
            //CultureInfo myCIclone = (CultureInfo)newCulture.Clone();
            //myCIclone.NumberFormat.NumberDecimalDigits = 2;
            //myCIclone.NumberFormat.NumberDecimalSeparator = ".";

            /*
             * PARA LAS FECHA PUEDEN USAR ESTE FORMATO QUE DEVUELVE LA FECHA EN UN STRING CORTO - -------Fecha.ToShortDateString()
             * PARA LAS HORAS PUEDEN USAR ESTE FORMATO QUE DEVUELVE LA FECHA EN UN STRING CORTO - -------Fecha.ToShortTimeString()
             * Campos2[long2, 0] EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS
             * Campos2[long2, 1] ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
             * Campos2[long2, 2] ES EL TIPO DE DATOS SEGUN LA TABLA ANALIZADA tablaInsert
             */
            bool banderaTransaccion = true;
            try
            {
                DataTable tmpMySqlInsertC = new DataTable();

                tmpMySqlInsertC = MYSQLSelect(tipoconexion, "select * from " + tablaInsert + " Limit 1", "1");

                int longitud = Dict_Insert.Count; // Contamos cuantos objectos tiene el diccionario
                int long2 = 0;
                string xMySqlInsertcodigo = "";
                //Multidimensional Arrays - Matrices Multidimensional 
                string[,] Campos2 = new string[longitud, 3];
                bool numerico = false;
                decimal Valor_numerico = 0;


                foreach (KeyValuePair<string, object> oDict in Dict_Insert) //Recorremos el diccionario de datos KeyValuePair (PARA TRAER LA KEY Y VALUE DEL DICCIONARIO)
                {
                    var tipox = "";

                    DataColumnCollection columns = tmpMySqlInsertC.Columns; // aqui traemos la estructura de la tabla 
                    if (columns.Contains(oDict.Key.ToLower().ToString().Trim())) // si las columnas contiene el campo en la tabla de base de datos
                    {
                        tipox = columns[oDict.Key.ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim();
                        Campos2[long2, 2] = columns[oDict.Key.ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim(); // ES EL TIPO DE VALOR QUE OBTENEMOS DE LA TABLA tablaInsert ()
                    }

                    int maxlength = -1;

                    //Si algun valor viene vacio o nulo lo vamos a omitir en el insert por lo cual si es un campo obligatoria no se podra poner en null o vacio
                    if (!Util.IsNullOrEmpty(oDict.Value))
                    {

                        Campos2[long2, 0] = oDict.Key.ToString().Trim(); // EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS

                        columns = tmpMySqlInsertC.Columns; // aqui traemos la estructura de la tabla 

                        /********************************BUSCAMOS EL TAMAÑO MAXIMO DE LA COLUMNA****************************************/
                        //Si no tuviera, deja el max length en -1 y omite recortar el dato


                        if (!Util.IsNullOrEmpty(columns[oDict.Key.ToLower().ToString().Trim()].MaxLength.ToString()))//Siempre y cuando tenga un tamaño maximo
                        {
                            maxlength = columns[oDict.Key.ToLower().ToString().Trim()].MaxLength;
                        }
                        else//Si no tiene, lo deja en -1
                        {
                            maxlength = -1;
                        }
                        /*Recortamos el valor a insertar*/
                        if (maxlength > 0)
                        {
                            Campos2[long2, 1] = Util.SUBSTR(oDict.Value.ToString().Trim(), 0, maxlength); // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
                        }
                        else
                        {
                            Campos2[long2, 1] = oDict.Value.ToString().Trim(); // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
                        }

                        /*************************************FIN TAMAÑO MAXIMO********************************************/

                        if (oDict.Key.ToString().Trim() == "codigo" && oDict.Value.GetType().Name == "String")
                        {
                            xMySqlInsertcodigo = oDict.Value.ToString().Trim();
                        }
                        long2 = long2 + 1;//SUMAMOS UNO PARA QUE EL ARRAY DIMENSIONAL CAMBIE A LA POSICION 2 

                    }
                    else if (Util.IsNullOrEmpty(oDict.Value) && tipox == "STRING")
                    {
                        Campos2[long2, 0] = oDict.Key.ToString().Trim(); // EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS
                        Campos2[long2, 1] = ""; // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
                        long2 = long2 + 1;//SUMAMOS UNO PARA QUE EL ARRAY DIMENSIONAL CAMBIE A LA POSICION 2 
                    }
                }
                //*******************************lleno las columnas********************
                longitud = long2; // sI declara logintud igual a la longitud del array campos2 o bien a el tamano del diccionario
                string sentencia_sql = "INSERT INTO " + tablaInsert + "("; // se declara la sentencia de inicio para proceder

                for (int i = 0; i < longitud; i++) //Se recorre el array
                {
                    if (!Util.IsNullOrEmpty(Campos2[i, 1]))//Preguntamos si el valor de la viene vacio o se encuentra lleno 
                    {
                        if (i == longitud - 1)
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0]; // Campos2[i, 0] EQUIVALENTE AL NOMBRE DE LA COLUMNA EN BASE DE DATOS
                        }
                        else
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0] + ","; // EQUIVALENTE AL NOMBRE DE LA COLUMNA EN BASE DE DATOS, SE CONCATENA CON LA , PARA LA  SIGUEINTE INSTRUCCION 
                        }
                    }
                    else if (Util.IsNullOrEmpty(Campos2[i, 1]) && Campos2[i, 2] == "STRING")//Preguntamos si el valor es string y está vacio lo mete
                    {
                        if (i == longitud - 1)
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0]; // Campos2[i, 0] EQUIVALENTE AL NOMBRE DE LA COLUMNA EN BASE DE DATOS
                        }
                        else
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0] + ","; // EQUIVALENTE AL NOMBRE DE LA COLUMNA EN BASE DE DATOS, SE CONCATENA CON LA , PARA LA  SIGUEINTE INSTRUCCION 
                        }
                    }
                    else//Si es vacio
                    {
                        if (i == longitud - 1)//Si es el último valor le elimina la coma
                        {
                            sentencia_sql = sentencia_sql.TrimEnd(',');//Elimina la ultima coma
                        }
                    }

                }
                sentencia_sql = sentencia_sql + ") VALUES ("; // SE CONCATENA LOS VALORES

                //*******************************lleno los campos (values)**********************
                string sentencia_bit = ""; // SE UTILIZA PARA HACER UNA COMPARTIVA DE CAMPO: VALOR -- ES VERIFICACION
                for (int i = 0; i < longitud; i++) //Recorremos la misma longitud, pero esta vez para llenar los campos de VALUES EJM -- insert into tablaInsert (x1,x2,x3) values ()
                {
                    if (!Util.IsNullOrEmpty(Campos2[i, 1]))//Siempre y cuando el valor sea diferente a vacio
                    {
                        string Valorx = "";
                        switch (Campos2[i, 2])
                        {
                            //Casos paquete de strings
                            case "STRING": // si es STRING ENTRA A ESTE CASO 
                            case "TIMESPAN": // si es TIMESPAN ENTRA A ESTE CASO 
                            case "BYTE[]": // si es STRING ENTRA A ESTE CASO 
                                Valorx = Campos2[i, 1].Replace("'", "");
                                Valorx = "'" + Valorx + "'";
                                sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                                break;
                            // Casos paquete de LOGICOS
                            case "BOOLEAN":
                                if (Campos2[i, 1] == "true" || Campos2[i, 1] == "TRUE" || Campos2[i, 1] == "True" || Campos2[i, 1] == "1")
                                {
                                    Valorx = "1";
                                }
                                else
                                {
                                    Valorx = "0"; // valor en falso
                                }
                                sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                                break;

                            case "INT16":/*Valida si es lógico o númerico*/
                                /*Si es logico positivo*/
                                if (Campos2[i, 1] == "true" ||
                                    Campos2[i, 1] == "TRUE" ||
                                    Campos2[i, 1] == "True" ||
                                    Campos2[i, 1] == "1")
                                {
                                    Valorx = "1";
                                }
                                else /*Si es logico negativo*/
                                if (Campos2[i, 1] == "false" ||
                                   Campos2[i, 1] == "FALSE" ||
                                   Campos2[i, 1] == "False" ||
                                   Campos2[i, 1] == "0")
                                {
                                    Valorx = "0"; // valor en falso
                                }
                                else//Sino, es númerico
                                {
                                    if (Util.IsNullOrEmpty(Campos2[i, 1]))
                                    {
                                        Campos2[i, 1] = "0";
                                    }
                                    NumberStyles style_;
                                    CultureInfo culture_;
                                    decimal number_ = 0;
                                    style_ = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                                    culture_ = CultureInfo.CreateSpecificCulture("en-US");//Seteamos la cultura para trabajar con "."
                                    if (Decimal.TryParse(Campos2[i, 1], style_, culture_, out number_))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                                    {//Correcto es numerico
                                        numerico = true;// Bandera para no concantenar el valor a un string 
                                        Valorx = number_ + "";
                                        Valor_numerico = number_;
                                        sentencia_bit = sentencia_bit + Campos2[i, 0].Replace(",", ".") + " : " + number_ + " ";
                                    }
                                    else
                                    {
                                        decimal number2;
                                        string VALUE = Campos2[i, 1].ToString();
                                        if (Decimal.TryParse(Campos2[i, 1].ToString(), out number2))
                                            Console.WriteLine(number2);
                                        else
                                            culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
                                        if (/*!banderaTransaccion &&*/ tablaInsert.ToLower() != "bitacora_componente_sql")
                                        {
                                            /*Excepcion*/
                                            string cuerpo = Util.GetLine(Dict_Insert);
                                            Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                            /*Fin*/
                                        }

                                        return false;
                                    }
                                    culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
                                }

                                sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                                break;
                            // Casos paquete de NUMERICOS
                            case "DOUBLE":
                                Valorx = callNumerales.ValidacionDouble(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(Dict_Insert);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    /*Fin*/
                                    return false;
                                }
                                break;
                            case "INT32":
                                Valorx = callNumerales.ValidacionInt32(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(Dict_Insert);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    /*Fin*/
                                    return false;
                                }
                                break;


                            case "INT64":
                                Valorx = callNumerales.ValidacionInt64(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(Dict_Insert);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    /*Fin*/
                                    return false;
                                }
                                break;
                            case "DECIMAL":
                                Valorx = callNumerales.ValidacionDecimal(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(Dict_Insert);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    /*Fin*/
                                    return false;
                                }
                                break;
                            case "FLOAT":
                                Valorx = callNumerales.ValidacionFloat(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(Dict_Insert);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    /*Fin*/
                                    return false;
                                }
                                break;
                            case "DATETIME":

                                Valorx = callFecha.ValidacionFecha(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(Dict_Insert);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, "Conversion de fecha:" + Campos2[i, 1]);
                                    /*Fin*/
                                }
                                break;

                                //default: return false;
                        }//fIN DEL SWITCH


                        if (i == longitud - 1)
                        {
                            sentencia_sql = sentencia_sql + Valorx;//Si es el último dato no le concatenamos ,
                        }
                        else
                        {
                            sentencia_sql = sentencia_sql + Valorx + ",";//Si no, si le concatenamos ,
                        }
                    }
                    else if (Campos2[i, 2] == "STRING" && Util.IsNullOrEmpty(Campos2[i, 1]))//Si fuera de tipo String y está vacío
                    {
                        string Valorx = "''";//Inserta el valor en vacio

                        if (i == longitud - 1)
                        {
                            sentencia_sql = sentencia_sql + Valorx;//Si es el último dato no le concatenamos ,
                        }
                        else
                        {
                            sentencia_sql = sentencia_sql + Valorx + ",";//Si no, si le concatenamos ,
                        }
                    }
                    else//Si es vacio y de otro tipo que no sea string
                    {
                        if (i == longitud - 1)//Si es el último valor le elimina la coma
                        {
                            sentencia_sql = sentencia_sql.TrimEnd(',');//Elimina la ultima coma
                        }
                    }
                }//FIN DEL FOR DE LLANADO DE VALUES
                sentencia_sql = sentencia_sql + ");";

                /*ESTE BLOQUE NOS DEVUELVE EL STRING DE CONEXION */
                try
                {
                    string mySqlSelectconex = "";
                    int johexitcant = 0;
                    int johexitbandera = 0;

                    //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
                    while (johexitbandera < 1 && johexitcant < 10)
                    {
                        mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
                        if (!string.IsNullOrEmpty(mySqlSelectconex))
                        {
                            if (!mySqlSelectconex.Contains("error"))
                            {
                                johexitbandera = 1;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                            //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
                        }
                        else
                        {
                            Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
                        }
                        johexitcant = johexitcant + 1;
                    }

                    if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex) && !string.IsNullOrEmpty(sentencia_sql))
                    {
                        //AQUI HACEMOS LA INSERCCION DE MYSQL
                        banderaTransaccion = conexion.execute_Mysql(sentencia_sql, mySqlSelectconex);
                    }
                    else
                    {
                        banderaTransaccion = false;
                    }

                    if (!banderaTransaccion && tablaInsert.ToLower() != "bitacora_componente_sql")
                    {
                        //Si se cae el query insertamos en bitacora
                        //Mientras sea diferente de la bitacora, para no enciclar
                        Util.Bitacora_ComponenteSQL(Usuario, sentencia_sql, banderaTransaccion, "MYSQL-" + tipoconexion, metodo, contrato);
                    }

                }
                catch (Exception exc)
                {
                    if (/*!banderaTransaccion &&*/ tablaInsert.ToLower() != "bitacora_componente_sql")
                    {
                        /*Excepcion*/

                        string cuerpo = Util.GetLine(Dict_Insert);

                        Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, exc.ToString());

                        /*Fin*/
                    }

                    string error = exc.ToString();
                    banderaTransaccion = false;
                }
            }
            catch (Exception e)
            {
                if (/*!banderaTransaccion &&*/ tablaInsert.ToLower() != "bitacora_componente_sql")
                {
                    /*Excepcion*/
                    string cuerpo = Util.GetLine(Dict_Insert);
                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "INSERT", tablaInsert, Usuario, tipoconexion, "", metodo, contrato, e.ToString());
                    /*Fin*/
                }
                string error = e.ToString();
                banderaTransaccion = false;
            }
            return banderaTransaccion;
        }


        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo de Actualización a MSYQL
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados
        public bool MYSQLUpdate(string tipoconexion, string tablaUpdate, Dictionary<string, object> oDict_, Trictionary<string, object, string[]> oDict2_, string metodo, string contrato)
        {
            /*Usuario*/
            string Usuario = "";
            /*Fin Usuario*/

            /*
             * PARA LAS FECHA PUEDEN USAR ESTE FORMATO QUE DEVUELVE LA FECHA EN UN STRING CORTO - -------Fecha.ToShortDateString()
             * PARA LAS HORAS PUEDEN USAR ESTE FORMATO QUE DEVUELVE LA FECHA EN UN STRING CORTO - -------Fecha.ToShortTimeString()
             * Campos2[long2, 0] EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS
             * Campos2[long2, 1] ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
             * Campos2[long2, 2] ES EL TIPO DE DATOS SEGUN LA TABLA ANALIZADA tablaUpdate
             * Campos2[long2, 3] ES EL TIPO (= > < <> !=) DE LOS WHERE ---- oDict2_
             * Campos2[long2, 4] ES EL Operadores (AND-OR) DE LOS WHERE ---- oDict2_
             * OPERADOR ES 100 % OPCIONAL - SI SE MANDA VACIO SE UTILIZA EL OPERADOR AND Y SI NO SE DEBE ESPECIFICAR EL QUE SE VA A MANDAR (OR)
             */

            //newCulture = new CultureInfo("es-CR");
            //CultureInfo.DefaultThreadCurrentCulture = newCulture;
            bool banderaTransaccion = true;
            try
            {
                DataTable tmpMySqlInsertC = new DataTable();
                tmpMySqlInsertC = MYSQLSelect(tipoconexion, "select * from " + tablaUpdate + " Limit 1", "1");
                int longitud = oDict_.Count; // Contamos cuantos objectos tiene el diccionario
                int long2 = 0;
                //Multidimensional Arrays - Matrices Multidimensional 
                string[,] Campos2 = new string[longitud, 3];

                foreach (KeyValuePair<string, object> oDict in oDict_) //Recorremos el diccionario de datos KeyValuePair (PARA TRAER LA KEY Y VALUE DEL DICCIONARIO)
                {
                    var tipox = "";

                    DataColumnCollection columns = tmpMySqlInsertC.Columns; // aqui traemos la estructura de la tabla 
                    if (columns.Contains(oDict.Key.ToLower().ToString().Trim())) // si las columnas contiene el campo en la tabla de base de datos
                    {
                        tipox = columns[oDict.Key.ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim();
                        Campos2[long2, 2] = columns[oDict.Key.ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim(); // ES EL TIPO DE VALOR QUE OBTENEMOS DE LA TABLA tablaInsert ()
                    }

                    int maxlength = -1;

                    //Si algun valor viene vacio o nulo lo vamos a omitir en el insert por lo cual si es un campo obligatoria no se podra poner en null o vacio
                    if (!Util.IsNullOrEmpty(oDict.Value))
                    {

                        columns = tmpMySqlInsertC.Columns; // aqui traemos la estructura de la tabla 


                        Campos2[long2, 0] = oDict.Key.ToString().Trim(); // EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS

                        /********************************BUSCAMOS EL TAMAÑO MAXIMO DE LA COLUMNA****************************************/
                        //Si no tuviera, deja el max length en -1 y omite recortar el dato

                        if (!String.IsNullOrEmpty(columns[oDict.Key.ToLower().ToString().Trim()].MaxLength.ToString()))//Siempre y cuando tenga un tamaño maximo
                        {
                            maxlength = columns[oDict.Key.ToLower().ToString().Trim()].MaxLength;
                        }
                        else//Si no tiene, lo deja en -1
                        {
                            maxlength = -1;
                        }

                        /*Recortamos el valor a insertar*/
                        if (maxlength > 0)
                        {
                            Campos2[long2, 1] = Util.SUBSTR(oDict.Value.ToString().Trim(), 0, maxlength); // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
                        }
                        else
                        {
                            Campos2[long2, 1] = oDict.Value.ToString().Trim(); // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
                        }

                        /*************************************FIN TAMAÑO MAXIMO********************************************/

                        if (columns.Contains(oDict.Key.ToLower().ToString().Trim())) // si las columnas contiene el campo en la tabla de base de datos
                        {
                            Campos2[long2, 2] = columns[oDict.Key.ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim(); // ES EL TIPO DE VALOR QUE OBTENEMOS DE LA TABLA tablaInsert ()
                        }


                        long2 = long2 + 1;//SUMAMOS UNO PARA QUE EL ARRAY DIMENSIONAL CAMBIE A LA POSICION 2 

                    }
                    else if (Util.IsNullOrEmpty(oDict.Value) && tipox == "STRING")//Si fuera vacio y es string
                    {
                        Campos2[long2, 0] = oDict.Key.ToString().Trim(); // EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS
                        Campos2[long2, 1] = ""; // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS                    

                        long2 = long2 + 1;//SUMAMOS UNO PARA QUE EL ARRAY DIMENSIONAL CAMBIE A LA POSICION 2 
                    }
                }

                if (oDict2_.Count <= 0)//Si no tiene ningún dato en el where lo devuelve
                {
                    string cuerpo = Util.GetLine(oDict_);

                    string where = Util.GetLine_Triccionary(oDict2_);

                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "No tiene where");

                    return false;
                }

                //*******************************lleno las columnas********************
                string sentencia_sql = "update " + tablaUpdate + " set ";
                string sentencia_bit = "";
                for (int i = 0; i < longitud; i++) //Recorremos la misma longitud, pero esta vez para llenar los campos de VALUES EJM -- insert into tablaInsert (x1,x2,x3) values ()
                {
                    if (!Util.IsNullOrEmpty(Campos2[i, 1]))
                    {
                        string Valorx = "";



                        switch (Campos2[i, 2])
                        {
                            //Casos paquete de strings
                            case "STRING": // si es STRING ENTRA A ESTE CASO 
                            case "TIMESPAN": // si es TIMESPAN ENTRA A ESTE CASO 
                            case "BYTE[]": // si es STRING ENTRA A ESTE CASO 

                                Valorx = Campos2[i, 1].Replace("'", "");
                                Valorx = "'" + Valorx + "'";
                                sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                                break;
                            // Casos paquete de LOGICOS
                            case "BOOLEAN":

                                if (Campos2[i, 1] == "true" || Campos2[i, 1] == "TRUE" || Campos2[i, 1] == "True" || Campos2[i, 1] == "1")
                                {
                                    Valorx = "1";
                                }
                                else
                                {
                                    Valorx = "0"; // valor en falso

                                }

                                sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                                break;
                            case "INT16":/*Valida si es lógico o númerico*/

                                /*Si es logico positivo*/
                                if (Campos2[i, 1] == "true" ||
                                    Campos2[i, 1] == "TRUE" ||
                                    Campos2[i, 1] == "True" ||
                                    Campos2[i, 1] == "1")
                                {
                                    Valorx = "1";
                                }
                                else /*Si es logico negativo*/
                                if (Campos2[i, 1] == "false" ||
                                   Campos2[i, 1] == "FALSE" ||
                                   Campos2[i, 1] == "False" ||
                                   Campos2[i, 1] == "0")
                                {
                                    Valorx = "0"; // valor en falso

                                }
                                else//Sino, es númerico
                                {

                                    NumberStyles style_;
                                    CultureInfo culture_;
                                    decimal number_ = 0;

                                    style_ = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                                    culture_ = CultureInfo.CreateSpecificCulture("en-US");//Seteamos la cultura para trabajar con "."
                                    if (Decimal.TryParse(Campos2[i, 1], style_, culture_, out number_))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                                    {//Correcto es numerico

                                        Valorx = number_ + "";
                                        /*Previene que el divisor cambie*/
                                        //Valorx = Valorx.Replace(",", ".");
                                        sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + number_ + " ";
                                    }
                                    else
                                    {
                                        culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
                                        /*Excepcion*/
                                        string cuerpo = Util.GetLine(oDict_);
                                        string where = Util.GetLine_Triccionary(oDict2_);
                                        Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                        /*Fin*/
                                        return false;
                                    }
                                    culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
                                }
                                sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                                break;
                            // Casos paquete de NUMERICOS
                            case "DOUBLE":
                                Valorx = callNumerales.ValidacionDouble(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(oDict_);
                                    string where = Util.GetLine_Triccionary(oDict2_);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    return false;
                                }
                                break;
                            case "INT32":
                                Valorx = callNumerales.ValidacionInt32(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(oDict_);
                                    string where = Util.GetLine_Triccionary(oDict2_);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    return false;
                                }
                                break;
                            case "INT64":
                                Valorx = callNumerales.ValidacionInt64(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(oDict_);
                                    string where = Util.GetLine_Triccionary(oDict2_);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    return false;
                                }
                                break;
                            case "DECIMAL":
                                Valorx = callNumerales.ValidacionDecimal(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(oDict_);
                                    string where = Util.GetLine_Triccionary(oDict2_);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    return false;
                                }
                                break;
                            case "FLOAT":
                                Valorx = callNumerales.ValidacionFloat(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(oDict_);
                                    string where = Util.GetLine_Triccionary(oDict2_);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                    return false;
                                }
                                break;
                            //Case DATETIME 
                            case "DATETIME":
                                Valorx = callFecha.ValidacionFecha(Campos2[i, 1]);
                                if (string.IsNullOrEmpty(Valorx))
                                {
                                    string cuerpo = Util.GetLine(oDict_);
                                    string where = Util.GetLine_Triccionary(oDict2_);
                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de fecha:" + Campos2[i, 1]);
                                }
                                break;
                                //default: return false;
                        }//FIN DEL SWITCH
                        if (i == longitud - 1)
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0] + "=" + Valorx;//Si es el último valor no se le concatena ,
                        }
                        else
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0] + "=" + Valorx + ",";//Si no, si se le concatena ,
                        }
                    }
                    else if (Campos2[i, 2] == "STRING" && Util.IsNullOrEmpty(Campos2[i, 1]))//Si fuera de tipo String y está vacío
                    {
                        string Valorx = "''";//Inserta el valor en vacio

                        if (i == longitud - 1)
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0] + "=" + Valorx;//Si es el último valor no se le concatena ,
                        }
                        else
                        {
                            sentencia_sql = sentencia_sql + Campos2[i, 0] + "=" + Valorx + ",";//Si no, si se le concatena ,
                        }
                    }
                    else//Si es vacio y no fuera de tipo string
                    {
                        if (i == longitud - 1)//Si es el último valor le elimina la coma
                        {
                            sentencia_sql = sentencia_sql.TrimEnd(',');//Elimina la ultima coma
                        }
                    }
                }//FIN DEL FOR DE LLAMADO DE VALUES

                /*********************************************** LLENADO DEL WHERE *********************************/
                longitud = oDict2_.Count;
                long2 = 0;
                sentencia_sql = sentencia_sql + " where ";
                Campos2 = new string[longitud, 5];
                string xMYSQLUpdatecodigo = "";

                for (int i = 0; i < longitud; i++)
                {
                    Campos2[long2, 0] = oDict2_.Keys[i].ToString().Trim();
                    Campos2[long2, 1] = oDict2_.Values[i].ToString().Trim();
                    DataColumnCollection columns = tmpMySqlInsertC.Columns;
                    if (columns.Contains(oDict2_.Keys[i].ToLower().ToString().Trim()))
                    {
                        Campos2[long2, 2] = columns[oDict2_.Keys[i].ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim(); // ES EL TIPO DE VALOR QUE OBTENEMOS DE LA TABLA 
                    }

                    //**Types
                    if (string.IsNullOrEmpty(oDict2_.Controls[i][0].ToString().Trim())) // Si es vacio el operador sera un = 
                    {
                        Campos2[long2, 3] = "=";
                    }
                    else
                    {
                        //Pongale el operador que me mandan en el trydicionario, le agregamos espacios a los lados
                        if (oDict2_.Controls[i][0].ToString().Trim() == "<>")//Si el operador es igual a <> cambielo a != que funciona para MYSQL
                        {
                            Campos2[long2, 3] = " != ";
                        }
                        else//Sino utilizamos el recibido
                        {
                            Campos2[long2, 3] = " " + oDict2_.Controls[i][0].ToString().Trim() + " ";
                        }
                    }

                    if (string.IsNullOrEmpty(oDict2_.Controls[i][1].ToString().Trim()))
                    { //Pongale el operador AND 
                        Campos2[long2, 4] = " AND ";
                    }
                    else
                    {
                        //Pongale el operador que me mandan en la variable STRING[], le agregamos espacios al los lados
                        Campos2[long2, 4] = " " + oDict2_.Controls[i][1].ToString().Trim() + " ";
                    }
                    long2++;//Sumamos uno al campo
                }

                // *******************************lleno las columnas del where********************
                longitud = long2;
                for (int i = 0; i < long2; i++)
                {
                    string Valorx = "";
                    switch (Campos2[i, 2])
                    {
                        //Casos paquete de strings
                        case "STRING": // si es STRING ENTRA A ESTE CASO 
                        case "TIMESPAN": // si es TIMESPAN ENTRA A ESTE CASO 
                        case "BYTE[]": // si es STRING ENTRA A ESTE CASO 

                            Valorx = Campos2[i, 1].Replace("'", "");
                            Valorx = "'" + Valorx + "'";
                            sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                            break;
                        // Casos paquete de LOGICOS
                        case "BOOLEAN":

                            if (Campos2[i, 1] == "true" || Campos2[i, 1] == "TRUE" || Campos2[i, 1] == "True" || Campos2[i, 1] == "1")
                            {
                                Valorx = "1";
                            }
                            else
                            {
                                Valorx = "0"; // valor en falso

                            }

                            sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                            break;
                        case "INT16":/*Valida si es lógico o númerico*/

                            /*Si es logico positivo*/
                            if (Campos2[i, 1] == "true" ||
                                Campos2[i, 1] == "TRUE" ||
                                Campos2[i, 1] == "True" ||
                                Campos2[i, 1] == "1")
                            {
                                Valorx = "1";
                            }
                            else /*Si es logico negativo*/
                            if (Campos2[i, 1] == "false" ||
                               Campos2[i, 1] == "FALSE" ||
                               Campos2[i, 1] == "False" ||
                               Campos2[i, 1] == "0")
                            {
                                Valorx = "0"; // valor en falso

                            }
                            else//Sino, es númerico
                            {

                                NumberStyles style_;
                                CultureInfo culture_;
                                decimal number_ = 0;

                                style_ = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                                culture_ = CultureInfo.CreateSpecificCulture("en-US");//Seteamos la cultura para trabajar con "."
                                if (Decimal.TryParse(Campos2[i, 1], style_, culture_, out number_))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                                {//Correcto es numerico

                                    Valorx = number_ + "";

                                    /*Previene que el divisor cambie*/
                                    //Valorx = Valorx.Replace(",", ".");
                                    sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + number_ + " ";

                                }
                                else
                                {
                                    //culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada

                                    /*Excepcion*/

                                    string cuerpo = Util.GetLine(oDict_);

                                    string where = Util.GetLine_Triccionary(oDict2_);

                                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);

                                    /*Fin*/

                                    return false;
                                }

                                //culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada

                            }

                            //Si es númerico, colocarle que si es NULL cambiarlo a 0. CONSULTAR CON JOHNNY ESTA OPCION YA QUE SI NO SE ESTARÍAN
                            //PERDIENDO DATOS EN LA MIGRACION
                            ////Campos2[i, 0] = "IFNULL(" + oDict2_.Keys[i].ToString().Trim() + ",0)";
                            ////sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                            break;
                        // Casos paquete de NUMERICOS

                        case "DOUBLE":
                            Valorx = callNumerales.ValidacionDouble(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string cuerpo = Util.GetLine(oDict_);
                                string where = Util.GetLine_Triccionary(oDict2_);
                                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "INT32":
                            Valorx = callNumerales.ValidacionInt32(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string cuerpo = Util.GetLine(oDict_);
                                string where = Util.GetLine_Triccionary(oDict2_);
                                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "INT64":
                            Valorx = callNumerales.ValidacionInt64(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string cuerpo = Util.GetLine(oDict_);
                                string where = Util.GetLine_Triccionary(oDict2_);
                                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "DECIMAL":
                            Valorx = callNumerales.ValidacionDecimal(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string cuerpo = Util.GetLine(oDict_);
                                string where = Util.GetLine_Triccionary(oDict2_);
                                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "FLOAT":
                            Valorx = callNumerales.ValidacionFloat(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string cuerpo = Util.GetLine(oDict_);
                                string where = Util.GetLine_Triccionary(oDict2_);
                                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;

                        case "DATETIME":

                            Valorx = callFecha.ValidacionFecha(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string cuerpo = Util.GetLine(oDict_);
                                string where = Util.GetLine_Triccionary(oDict2_);
                                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, "Conversion de fecha:" + Campos2[i, 1]);
                                /*Fin*/
                            }
                            break;
                            //default: return false;
                    }//FIN DEL SWITCH

                    if (i == 0)
                    {
                        sentencia_sql = sentencia_sql + Campos2[i, 0] + Campos2[i, 3] + Valorx;//Si es el último valor, no concatenamos ,
                    }
                    else
                    {
                        sentencia_sql = sentencia_sql + " " + Campos2[i, 4] + " " + Campos2[i, 0] + Campos2[i, 3] + Valorx;//Si no si concatenamos ,
                    }
                }

                /*ESTE BLOQUE NOS DEVUELVE EL STRING DE CONEXION */
                try
                {
                    string mySqlSelectconex = "";
                    int johexitcant = 0;
                    int johexitbandera = 0;
                    //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
                    while (johexitbandera < 1 && johexitcant < 10)
                    {
                        mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
                        if (!string.IsNullOrEmpty(mySqlSelectconex))
                        {
                            if (!mySqlSelectconex.Contains("error"))
                            {
                                johexitbandera = 1;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                            //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
                        }
                        else
                        {
                            Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
                        }
                        johexitcant = johexitcant + 1;
                    }

                    if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex) && !string.IsNullOrEmpty(sentencia_sql))
                    {
                        //AQUI HACEMOS LA INSERCCION DE MYSQL
                        banderaTransaccion = conexion.execute_Mysql(sentencia_sql, mySqlSelectconex);
                    }
                    else
                    {
                        banderaTransaccion = false;
                    }

                    if (!banderaTransaccion && tablaUpdate.ToLower() != "bitacora_componente_sql")
                    {
                        //Si se cae el query insertamos en bitacora
                        //Mientras sea diferente de la bitacora, para no enciclar
                        Util.Bitacora_ComponenteSQL(Usuario, sentencia_sql, banderaTransaccion, "MYSQL-" + tipoconexion, metodo, contrato);
                    }

                }
                catch (Exception exc)
                {
                    /*Excepcion*/
                    string cuerpo = Util.GetLine(oDict_);
                    string where = Util.GetLine_Triccionary(oDict2_);
                    Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, exc.ToString());
                    /*Fin*/
                    string error = exc.ToString();
                    banderaTransaccion = false;
                }
            }
            catch (Exception e)
            {
                /*Excepcion*/
                string cuerpo = Util.GetLine(oDict_);
                string where = Util.GetLine_Triccionary(oDict2_);
                Util.Bitacora_ComponenteSQL_EXECEP(cuerpo, "UPDATE", tablaUpdate, Usuario, tipoconexion, where, metodo, contrato, e.ToString());
                /*Fin*/
                return false;
            }
            return banderaTransaccion;
        }


        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM)
        //Descripción: Metodo de Eliminar a MSYQL
        //Fecha de creación: 16/06/2021
        //Fecha Modificación:
        //Cambios Realizados
        public bool MYSQLDelete(string tipoconexion, string tablaDelete, Trictionary<string, object, string[]> oDict, string metodo, string contrato)
        {

            /*Usuario*/
            string Usuario = "";
            /*Fin Usuario*/

            //newCulture = new CultureInfo("es-CR");
            //CultureInfo.DefaultThreadCurrentCulture = newCulture;

            bool banderaTransaccion = true;
            string sentencia_sql = "delete from " + tablaDelete;
            try
            {
                DataTable tmpMySqlInsertC = new DataTable();
                tmpMySqlInsertC = MYSQLSelect(tipoconexion, "select * from " + tablaDelete + " Limit 1", "1");
                int longitud = oDict.Count; // Contamos cuantos objectos tiene el diccionario
                int long2 = 0;
                string[,] Campos2 = new string[longitud, 5];
                string xcodigo = "";

                for (int i = 0; i < longitud; i++)
                {
                    Campos2[long2, 0] = oDict.Keys[i].ToString().Trim();//Campo de la tabla a insertar
                    Campos2[long2, 1] = oDict.Values[i].ToString().Trim();//Valor a borrar en el campo de la tabla
                    DataColumnCollection columns = tmpMySqlInsertC.Columns;
                    if (columns.Contains(oDict.Keys[i].ToLower().ToString().Trim()))
                    {
                        Campos2[long2, 2] = columns[oDict.Keys[i].ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim(); // ES EL TIPO DE VALOR QUE OBTENEMOS DE LA TABLA 
                    }

                    //**Types  Relacionales
                    if (string.IsNullOrEmpty(oDict.Controls[i][0].ToString().Trim())) // Si es vacio el operador sera un = 
                    {
                        Campos2[long2, 3] = "=";
                    }
                    else
                    {
                        //Pongale el operador que me mandan en el trydicionario
                        Campos2[long2, 3] = oDict.Controls[i][0].ToString().Trim();
                    }

                    //Operadores Logicos

                    if (string.IsNullOrEmpty(oDict.Controls[i][1].ToString().Trim()))
                    { //Pongale el operador AND 
                        Campos2[long2, 4] = " AND ";
                    }
                    else
                    {
                        //Pongale el operador que me mandan en la variable STRING[]
                        Campos2[long2, 4] = oDict.Controls[i][1].ToString().Trim();
                    }


                    if (oDict.Keys.ToString().Trim() == "codigo" && oDict.Values.GetType().Name == "STRING")
                    {
                        xcodigo = oDict.Values.ToString().Trim();
                    }


                    long2++;//Sumamos uno al campo
                }

                if (oDict.Count <= 0)//Si no tiene ningún dato en el where lo devuelve
                {

                    /*Excepcion*/

                    string where = Util.GetLine_Triccionary(oDict);

                    Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "No tiene where");

                    /*Fin*/

                    return false;
                }

                //******************************lleno las columnas********************
                longitud = long2;
                sentencia_sql = sentencia_sql + " where ";
                string sentencia_bit = "";
                //******************************CODIGO NO ACEPTA AND Y OR, TAMPOCO ACEPTA  '<', '>, ' <> ' POR EL MOMENTO  ******************************

                for (int i = 0; i < long2; i++)
                {
                    string Valorx = "";
                    switch (Campos2[i, 2])
                    {
                        //Casos paquete de strings
                        case "STRING": // si es STRING ENTRA A ESTE CASO 
                        case "TIMESPAN": // si es TIMESPAN ENTRA A ESTE CASO 
                        case "BYTE[]": // si es STRING ENTRA A ESTE CASO 

                            Valorx = Campos2[i, 1].Replace("'", "");
                            Valorx = "'" + Valorx + "'";
                            sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                            break;
                        // Casos paquete de LOGICOS
                        case "BOOLEAN":

                            if (Campos2[i, 1] == "true" || Campos2[i, 1] == "TRUE" || Campos2[i, 1] == "True" || Campos2[i, 1] == "1")
                            {
                                Valorx = "1";
                            }
                            else
                            {
                                Valorx = "0"; // valor en falso

                            }

                            sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                            break;
                        case "INT16":/*Valida si es lógico o númerico*/

                            /*Si es logico positivo*/
                            if (Campos2[i, 1] == "true" ||
                                Campos2[i, 1] == "TRUE" ||
                                Campos2[i, 1] == "True" ||
                                Campos2[i, 1] == "1")
                            {
                                Valorx = "1";
                            }
                            else /*Si es logico negativo*/
                            if (Campos2[i, 1] == "false" ||
                               Campos2[i, 1] == "FALSE" ||
                               Campos2[i, 1] == "False" ||
                               Campos2[i, 1] == "0")
                            {
                                Valorx = "0"; // valor en falso

                            }
                            else//Sino, es númerico
                            {

                                NumberStyles style_;
                                CultureInfo culture_;
                                decimal number_ = 0;

                                style_ = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                                culture_ = CultureInfo.CreateSpecificCulture("en-US");//Seteamos la cultura para trabajar con "."
                                if (Decimal.TryParse(Campos2[i, 1], style_, culture_, out number_))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                                {//Correcto es numerico

                                    Valorx = number_ + "";

                                    /*Previene que el divisor cambie*/
                                    //Valorx = Valorx.Replace(",", ".");
                                    sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + number_ + " ";

                                }
                                else
                                {
                                    //culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada

                                    /*Excepcion*/

                                    string where = Util.GetLine_Triccionary(oDict);

                                    Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);

                                    /*Fin*/

                                    return false;
                                }
                                //culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
                            }

                            //Si es númerico, colocarle que si es NULL cambiarlo a 0. CONSULTAR CON JOHNNY ESTA OPCION YA QUE SI NO SE ESTARÍAN
                            //PERDIENDO DATOS EN LA MIGRACION

                            //Campos2[i, 0] = "IFNULL(" + oDict.Keys[i].ToString().Trim() + ",0)";
                            //sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
                            break;
                        // Casos paquete de NUMERICOS
                        case "DOUBLE":
                            Valorx = callNumerales.ValidacionDouble(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string where = Util.GetLine_Triccionary(oDict);
                                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "INT32":
                            Valorx = callNumerales.ValidacionInt32(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string where = Util.GetLine_Triccionary(oDict);
                                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;

                        case "INT64":
                            Valorx = callNumerales.ValidacionInt64(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string where = Util.GetLine_Triccionary(oDict);
                                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "DECIMAL":
                            Valorx = callNumerales.ValidacionDecimal(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string where = Util.GetLine_Triccionary(oDict);
                                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "FLOAT":
                            Valorx = callNumerales.ValidacionFloat(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string where = Util.GetLine_Triccionary(oDict);
                                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de número:" + Campos2[i, 1]);
                                return false;
                            }
                            break;
                        case "DATETIME":
                            Valorx = callFecha.ValidacionFecha(Campos2[i, 1]);
                            if (string.IsNullOrEmpty(Valorx))
                            {
                                string where = Util.GetLine_Triccionary(oDict);
                                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, "Conversion de fecha:" + Campos2[i, 1]);
                                /*Fin*/
                            }
                            break;
                    }//FIN DEL SWITCH

                    if (i == 0)
                    {
                        sentencia_sql = sentencia_sql + Campos2[i, 0] + Campos2[i, 3] + Valorx;//Si es el último valor no concatenamos ,
                    }
                    else
                    {
                        sentencia_sql = sentencia_sql + " " + Campos2[i, 4] + " " + Campos2[i, 0] + Campos2[i, 3] + Valorx;//Si no si concatenamos ,
                    }

                }//FIN DEL FOR

                /*ESTE BLOQUE NOS DEVUELVE EL STRING DE CONEXION */
                try
                {
                    string mySqlSelectconex = "";
                    int johexitcant = 0;
                    int johexitbandera = 0;
                    //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
                    while (johexitbandera < 1 && johexitcant < 10)
                    {
                        mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
                        if (!string.IsNullOrEmpty(mySqlSelectconex))
                        {
                            if (!mySqlSelectconex.Contains("error"))
                            {
                                johexitbandera = 1;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                            //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
                        }
                        else
                        {
                            Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
                        }
                        johexitcant = johexitcant + 1;
                    }



                    if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex) && !string.IsNullOrEmpty(sentencia_sql))
                    {
                        //AQUI HACEMOS LA INSERCCION DE MYSQL
                        banderaTransaccion = conexion.execute_Mysql(sentencia_sql, mySqlSelectconex);
                    }
                    else
                    {
                        banderaTransaccion = false;
                    }

                    if (!banderaTransaccion && tablaDelete.ToLower() != "bitacora_componente_sql")
                    {
                        //Si se cae el query insertamos en bitacora
                        //Mientras sea diferente de la bitacora, para no enciclar
                        Util.Bitacora_ComponenteSQL(Usuario, sentencia_sql, banderaTransaccion, "MYSQL-" + tipoconexion, metodo, contrato);
                    }

                }
                catch (Exception exc)
                {
                    /*Excepcion*/

                    string where = Util.GetLine_Triccionary(oDict);

                    Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, exc.ToString());

                    /*Fin*/

                    string error = exc.ToString();
                    banderaTransaccion = false;
                }
            }
            catch (Exception ex)
            {

                /*Excepcion*/

                string where = Util.GetLine_Triccionary(oDict);

                Util.Bitacora_ComponenteSQL_EXECEP("", "DELETE", tablaDelete, Usuario, tipoconexion, where, metodo, contrato, ex.ToString());

                /*Fin*/

                return false;
            }
            return banderaTransaccion;

        }



        //Nombre: Componente_sql
        //Creado por: Telecable(BJZ)
        //Descripción: Metodo encargado de ejecutar directamente
        //Fecha de creación: 30/08/2021
        //Fecha Modificación:
        //Cambios Realizados:

        public bool MYSQLDirecto(string tipoconexion, string tabla, string query, string metodo, string contrato)
        {
            /*Usuario*/
            string Usuario = "";
            /*Fin Usuario*/

            bool banderaTransaccion = true;


            /*ESTE BLOQUE NOS DEVUELVE EL STRING DE CONEXION */
            try
            {
                string mySqlSelectconex = "";
                int johexitcant = 0;
                int johexitbandera = 0;
                //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
                while (johexitbandera < 1 && johexitcant < 10)
                {
                    mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
                    if (!string.IsNullOrEmpty(mySqlSelectconex))
                    {
                        if (!mySqlSelectconex.Contains("error"))
                        {
                            johexitbandera = 1;
                        }
                        else
                        {
                            Thread.Sleep(1000);
                        }
                        //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
                    }
                    else
                    {
                        Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
                    }
                    johexitcant = johexitcant + 1;
                }



                if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex) && !string.IsNullOrEmpty(query))
                {
                    //AQUI HACEMOS LA INSERCCION DE MYSQL
                    banderaTransaccion = conexion.execute_Mysql(query, mySqlSelectconex);

                }
                else
                {
                    banderaTransaccion = false;
                }

                if (!banderaTransaccion && !query.ToLower().Contains("bitacora_componente_sql"))
                {
                    //Si se cae el query insertamos en bitacora
                    //Mientras sea diferente de la bitacora, para no enciclar
                    Util.Bitacora_ComponenteSQL(Usuario, query, banderaTransaccion, "MYSQL-" + tipoconexion, metodo, contrato);
                }
            }
            catch (Exception exc)
            {
                string error = exc.ToString();
                banderaTransaccion = false;

                if (/*!banderaTransaccion &&*/ !query.ToLower().Contains("bitacora_componente_sql"))
                {
                    //Si se cae el query insertamos en bitacora
                    //Mientras sea diferente de la bitacora, para no enciclar
                    Util.Bitacora_ComponenteSQL(Usuario, query, banderaTransaccion, "MYSQL-" + tipoconexion, metodo, contrato);
                }

            }

            return banderaTransaccion;


        }



        //Nombre: Componente_sql
        //Creado por: Telecable(JHH, GEM, BJZ)
        //Descripción: Metodo de Insercion Masivo
        //Fecha de creación: 09/06/2021
        //Fecha Modificación:
        //Cambios Realizados:
        //public bool MYSQLMASIVO(string tipoconexion, string tablaInsert, DataTable Dict_Insert_Masivo)
        //{
        //    //newCulture = new CultureInfo("en-US");
        //    //CultureInfo.DefaultThreadCurrentCulture = newCulture;
        //    //CultureInfo myCIclone = (CultureInfo)newCulture.Clone();
        //    //myCIclone.NumberFormat.NumberDecimalDigits = 2;
        //    //myCIclone.NumberFormat.NumberDecimalSeparator = ".";




        //    /*
        //     * PARA LAS FECHA PUEDEN USAR ESTE FORMATO QUE DEVUELVE LA FECHA EN UN STRING CORTO - -------Fecha.ToShortDateString()
        //     * PARA LAS HORAS PUEDEN USAR ESTE FORMATO QUE DEVUELVE LA FECHA EN UN STRING CORTO - -------Fecha.ToShortTimeString()
        //     * Campos2[long2, 0] EQUIVALENTE AL NOMBRE DE LA TABLA EN BASE DE DATOS
        //     * Campos2[long2, 1] ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS
        //     * Campos2[long2, 2] ES EL TIPO DE DATOS SEGUN LA TABLA ANALIZADA tablaInsert
        //     */

        //    //Armamos el Diccionario de Datos para la inserción de una linea

        //    Dictionary<string, object> Dict_Insert = new Dictionary<string, object>();// Diccionario para insertar, modificar valores

        //    bool banderaTransaccion = true;


        //    //DataTable Reporte_Masivo = new DataTable();
        //    //Reporte_Masivo.Clear();

        //    //Reporte_Masivo.Columns.Add("Inicio-Fin");
        //    //Reporte_Masivo.Columns.Add("Estado");
        //    //Reporte_Masivo.Columns.Add("Duracion");


        //    //DataRow addrow_Reporte_Masivo;




        //    //Console.WriteLine("INICIANDO INSERCIÓN MASIVA A TABLA: " + tablaInsert);

        //    //Stopwatch Tiempo_Total = new Stopwatch();
        //    //Tiempo_Total.Start();

        //    int inicio = 1;
        //    int fin = 0;

        //    try
        //    {
        //        int Linea = 1;//Contador actual de lineas

        //        int Lineas_Totales = Dict_Insert_Masivo.Rows.Count;//Total de lineas

        //        /*LLENAMOS EL ENCABEZADO DEL QUERY*/

        //        string sentencia_sql_encabezado = "INSERT INTO " + tablaInsert + "("; // se declara la sentencia de inicio para proceder

        //        int contador_columnas = 0;

        //        foreach (DataColumn column in Dict_Insert_Masivo.Columns)
        //        {

        //            if (contador_columnas == (Dict_Insert_Masivo.Columns.Count - 1))
        //            {
        //                sentencia_sql_encabezado = sentencia_sql_encabezado + column.ColumnName.ToString();//Insertamos por columna y valor
        //            }
        //            else
        //            {
        //                sentencia_sql_encabezado = sentencia_sql_encabezado + column.ColumnName.ToString() + ","; // EQUIVALENTE AL NOMBRE DE LA COLUMNA EN BASE DE DATOS, SE CONCATENA CON LA , PARA LA  SIGUEINTE INSTRUCCION 
        //            }

        //            contador_columnas++;
        //        }



        //        sentencia_sql_encabezado = sentencia_sql_encabezado + ") VALUES "; // SE CONCATENA LOS VALORES

        //        string sentencia_sql = "";

        //        sentencia_sql = sentencia_sql_encabezado;//Concatenamos el encabezado

        //        int cont_values = 1;


        //        //Stopwatch Tiempo_X_Linea = new Stopwatch();
        //        //Tiempo_X_Linea.Start();


        //        foreach (DataRow row in Dict_Insert_Masivo.Rows)
        //        {
        //            string values = "(";
        //            //Inicia el contador de insercion de linea



        //            Console.WriteLine("INICIANDO SENTENCIA DE INSERT LINEA: " + Linea + " DE: " + Lineas_Totales + " LINEAS");

        //            Dict_Insert.Clear();//Cada vez que empieza una nueva linea, limpia el Diccionario

        //            foreach (DataColumn column in Dict_Insert_Masivo.Columns)
        //            {

        //                Dict_Insert.Add(column.ColumnName.ToString(), row[column].ToString());//Insertamos por columna y valor

        //            }

        //            /*************************************UNA VEZ CONSTRUIDO EL DICCIONARIO*****************************************/

        //            //DataTable tmpMySqlInsertC = new DataTable();   

        //            int longitud = Dict_Insert.Count; // Contamos cuantos objectos tiene el diccionario
        //            int long2 = 0;
        //            string xMySqlInsertcodigo = "";
        //            //Multidimensional Arrays - Matrices Multidimensional 
        //            string[,] Campos2 = new string[longitud, 3];
        //            bool numerico = false;
        //            decimal Valor_numerico = 0;




        //            foreach (KeyValuePair<string, object> oDict in Dict_Insert) //Recorremos el diccionario de datos KeyValuePair (PARA TRAER LA KEY Y VALUE DEL DICCIONARIO)
        //            {

        //                //Si algun valor viene vacio o nulo lo vamos a omitir en el insert por lo cual si es un campo obligatoria no se podra poner en null o vacio
        //                //if (!string.IsNullOrEmpty(oDict.Value.ToString().Trim()))
        //                //{

        //                Campos2[long2, 0] = oDict.Key.ToString().Trim(); // EQUIVALENTE AL NOMBRE DE LA COLUMNA EN BASE DE DATOS

        //                Campos2[long2, 1] = oDict.Value.ToString().Trim(); // ES EL VALOR QUE INTRODUCIRA EN LA BASE DE DATOS

        //                /*Del DataTable que enviamos verifica que tipo de Dato estamos enviando*/

        //                DataColumnCollection columns = Dict_Insert_Masivo.Columns; // aqui traemos la estructura de la tabla

        //                if (columns.Contains(oDict.Key.ToLower().ToString().Trim())) // si las columnas contiene el campo en la tabla de base de datos
        //                {
        //                    Campos2[long2, 2] = columns[oDict.Key.ToLower().ToString().Trim()].DataType.Name.ToUpper().Trim(); // ES EL TIPO DE VALOR QUE OBTENEMOS DE LA TABLA tablaInsert ()
        //                }

        //                /*Fin validacion de datos*/

        //                long2 = long2 + 1;//SUMAMOS UNO PARA QUE EL ARRAY DIMENSIONAL CAMBIE A LA POSICION 2 

        //                //}

        //            }
        //            //*******************************lleno las columnas********************
        //            longitud = long2; // sI declara logintud igual a la longitud del array campos2 o bien a el tamano del diccionario



        //            //*******************************lleno los campos (values)**********************
        //            string sentencia_bit = ""; // SE UTILIZA PARA HACER UNA COMPARTIVA DE CAMPO: VALOR -- ES VERIFICACION
        //            for (int i = 0; i < longitud; i++) //Recorremos la misma longitud, pero esta vez para llenar los campos de VALUES EJM -- insert into tablaInsert (x1,x2,x3) values ()
        //            {
        //                //if (!string.IsNullOrEmpty(Campos2[i, 1]))//Siempre y cuando el valor sea diferente a vacio
        //                //{
        //                string Valorx = "";

        //                #region Conversiones
        //                switch (Campos2[i, 2])
        //                {
        //                    //Casos paquete de strings
        //                    case "STRING": // si es STRING ENTRA A ESTE CASO 
        //                    case "TIMESPAN": // si es TIMESPAN ENTRA A ESTE CASO 
        //                    case "BYTE[]": // si es STRING ENTRA A ESTE CASO 

        //                        Valorx = Util.EliminarCaracteresEspeciales(Campos2[i, 1]);

        //                        if (Util.IsNullOrEmpty(Campos2[i, 1]))//Si viene vacio insertamos empty
        //                        {
        //                            Valorx = "";
        //                        }

        //                        Valorx = "'" + Valorx + "'";
        //                        sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
        //                        break;
        //                    // Casos paquete de LOGICOS
        //                    case "BOOLEAN":

        //                        if (Campos2[i, 1] == "true" || Campos2[i, 1] == "TRUE" || Campos2[i, 1] == "True" || Campos2[i, 1] == "1")
        //                        {
        //                            Valorx = "1";
        //                        }
        //                        else
        //                        {
        //                            Valorx = "0"; // valor en falso

        //                        }

        //                        sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
        //                        break;

        //                    case "INT16":/*Valida si es lógico o númerico*/

        //                        /*Si es logico positivo*/
        //                        if (Campos2[i, 1] == "true" ||
        //                            Campos2[i, 1] == "TRUE" ||
        //                            Campos2[i, 1] == "True" ||
        //                            Campos2[i, 1] == "1")
        //                        {
        //                            Valorx = "1";
        //                        }
        //                        else /*Si es logico negativo*/
        //                        if (Campos2[i, 1] == "false" ||
        //                           Campos2[i, 1] == "FALSE" ||
        //                           Campos2[i, 1] == "False" ||
        //                           Campos2[i, 1] == "0" ||
        //                           Util.IsNullOrEmpty(Campos2[i, 1]))
        //                        {
        //                            Valorx = "0"; // valor en falso

        //                        }
        //                        else//Sino, es númerico
        //                        {
        //                            NumberStyles style_;
        //                            CultureInfo culture_;
        //                            decimal number_ = 0;

        //                            style_ = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        //                            culture_ = CultureInfo.CreateSpecificCulture("en-US");//Seteamos la cultura para trabajar con "."
        //                            if (Decimal.TryParse(Campos2[i, 1], style_, culture_, out number_))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
        //                            {//Correcto es numerico
        //                                numerico = true;// Bandera para no concantenar el valor a un string 
        //                                Valorx = number_ + "";
        //                                Valor_numerico = number_;

        //                                /*Previene que se cambie el separador*/
        //                                //Valorx = Valorx.Replace(",", ".");

        //                                sentencia_bit = sentencia_bit + Campos2[i, 0].Replace(",", ".") + " : " + number_ + " ";

        //                            }
        //                            else
        //                            {
        //                                decimal number2;
        //                                string VALUE = Campos2[i, 1].ToString();
        //                                if (Decimal.TryParse(Campos2[i, 1].ToString(), out number2))
        //                                    Console.WriteLine(number2);
        //                                else
        //                                    Console.WriteLine("Unable to parse '{0}'.", Campos2[i, 1]);
        //                                culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
        //                                return false;
        //                            }
        //                            culture_ = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
        //                        }

        //                        sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Campos2[i, 1] + " ";
        //                        break;
        //                    // Casos paquete de NUMERICOS
        //                    case "DOUBLE":
        //                    case "INT32":
        //                    case "INT64":
        //                    case "DECIMAL":
        //                    case "FLOAT":


        //                        /*FORMATEAR VALORES DE DOUBLE O INT A NUMERICOS PARA PODER INSERTAR EN LA BASE DE DATOS
        //                         * 
        //                         * https://docs.microsoft.com/en-us/dotnet/api/system.decimal.tryparse?view=net-5.0#code-try-2  
        //                         *
        //                         */


        //                        NumberStyles style;
        //                        CultureInfo culture;
        //                        decimal number = 0;

        //                        string num_temp;

        //                        if (Util.IsNullOrEmpty(Campos2[i, 1]))
        //                        {
        //                            num_temp = "0";
        //                        }
        //                        else
        //                        {
        //                            num_temp = Campos2[i, 1];
        //                        }

        //                        style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
        //                        culture = CultureInfo.CreateSpecificCulture("en-US");//Seteamos la cultura para trabajar con "."
        //                        if (Decimal.TryParse(num_temp, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
        //                        {//Correcto es numerico
        //                            numerico = true;// Bandera para no concantenar el valor a un string 
        //                            Valorx = number + "";
        //                            Valor_numerico = number;

        //                            /*Previene que se cambie el separador*/
        //                            //Valorx = Valorx.Replace(",",".");

        //                            sentencia_bit = sentencia_bit + num_temp.Replace(",", ".") + " : " + number + " ";

        //                        }
        //                        else
        //                        {
        //                            if (Campos2[i, 1].Contains(",") && Campos2[i, 1].Contains("."))//Si el número dio error y contiene esos dos simbolos(, y .)
        //                            {
        //                                string Temp_Numero = Campos2[i, 1];

        //                                Temp_Numero = Temp_Numero.Replace(",", "%");//Reemplazamos las , por un simbolo temporal

        //                                Temp_Numero = Temp_Numero.Replace(".", "$");//Reemplazamos los . por un simbolo temporal

        //                                /************************************LIMPIAMOS EL NÚMERO******************************************/

        //                                Temp_Numero = Temp_Numero.Replace("%", ".");//Reemplazamos el simbolo temporal por el punto %

        //                                Temp_Numero = Temp_Numero.Replace("$", ",");//Reemplazamos el simbolo temporal por la coma $

        //                                /**************************REINTENTAMOS LA CONVERSIÓN**********************************/
        //                                if (Decimal.TryParse(Temp_Numero, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
        //                                {//Correcto es numerico
        //                                    numerico = true;// Bandera para no concantenar el valor a un string 
        //                                    Valorx = number + "";
        //                                    Valor_numerico = number;

        //                                    /*Previene que se cambie el separador*/
        //                                    //Valorx = Valorx.Replace(",", ".");

        //                                    sentencia_bit = sentencia_bit + Campos2[i, 0].Replace(",", ".") + " : " + number + " ";

        //                                    break;

        //                                }

        //                            }

        //                            decimal number2;
        //                            string VALUE = Campos2[i, 1].ToString();
        //                            if (Decimal.TryParse(Campos2[i, 1].ToString(), out number2))
        //                                Console.WriteLine(number);
        //                            else
        //                                Console.WriteLine("Unable to parse '{0}'.", Campos2[i, 1]);
        //                            culture = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
        //                            return false;
        //                        }
        //                        culture = CultureInfo.CreateSpecificCulture("es-CR");//Devolvemos a la cultura adecuada
        //                        break;

        //                    //Case DATETIME 
        //                    case "DATETIME":
        //                        DateTime FECHA_FORMAT;

        //                        //string[] formats = { "dd/M/yyyy HH:mm:ss"  };//Formato de la fecha

        //                        /*
        //                         Formatos Aceptados
        //                         Dia/Mes/Año Hora:minutos:segundos-------------- Escenario Ideal
        //                         Dia/Mes/Año -------------- Escenario Ideal
        //                         Año/Mes/dia 

        //                         */

        //                        /*Validacion de tt, C# no admite el formato con puntos! Limpiamos la fecha si viene así*/
        //                        var Date = "";//Almacena la fecha

        //                        if (Campos2[i, 1].Contains("a.m.") || Campos2[i, 1].Contains("a. m."))
        //                        {
        //                            Date = Campos2[i, 1].Replace("a.m.", "AM");
        //                            Date = Campos2[i, 1].Replace("a. m.", "AM");
        //                        }
        //                        else if (Campos2[i, 1].Contains("p.m.") || Campos2[i, 1].Contains("p. m."))
        //                        {
        //                            Date = Campos2[i, 1].Replace("p.m.", "PM");
        //                            Date = Campos2[i, 1].Replace("p. m.", "PM");
        //                        }
        //                        else//Si no contiene ninguno
        //                        {
        //                            Date = Campos2[i, 1];
        //                        }



        //                        /*Fin validacion*/

        //                        string[] formats = { 
        //                        //DIA/MES/ANNO
        //                        "dd/M/yyyy", "dd/M/yyyy HH:mm:ss", "d/M/yyyy", "d/M/yyyy h:mm:ss", "d/M/yyyy h:mm:ss tt", "dd/MM/yyyy hh:mm:ss",
        //                        "dd/MM/yyyy",
        //                        "d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt","d/M/yyyy hh:mm tt","d/M/yyyy hh tt",
        //                        "d/M/yyyy h:mm","d/M/yyyy h:mm","dd/MM/yyyy hh:mm", "dd/M/yyyy hh:mm",
        //                        //Nuevo
        //                        "d/M/yyyy HH:mm:ss",
        //                        //DIA-MES-ANNO
        //                        "dd-M-yyyy", "dd-M-yyyy HH:mm:ss", "d-M-yyyy", "d-M-yyyy h:mm:ss", "d-M-yyyy h:mm:ss tt", "dd-MM-yyyy hh:mm:ss",
        //                        "dd-MM-yyyy",
        //                        "d-M-yyyy h:mm:ss tt", "d-M-yyyy h:mm tt","d-M-yyyy hh:mm tt","d-M-yyyy hh tt",
        //                        "d-M-yyyy h:mm","d-M-yyyy h:mm","dd-MM-yyyy hh:mm", "dd-M-yyyy hh:mm",
        //                        //Nuevo
        //                        "d-M-yyyy HH:mm:ss"


        //                         };
        //                        if (DateTime.TryParseExact(Date, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
        //                        {//si EXISTE UN FORMATO VALIDO 
        //                         //Si la fecha es valida entonces se remplaza el formato a yyyy/MM/dd para insertar en MYSQL
        //                            string Fecha_new = FECHA_FORMAT.ToString("yyyy/MM/dd HH:mm:ss");

        //                            //SE Agregar el valor
        //                            Valorx = Fecha_new;

        //                            string Fecha_Temp = Util.SUBSTR(Valorx, 0, 10);

        //                            if (Fecha_Temp == "1899/12/30" || Fecha_Temp == "1899/12/31" || Fecha_Temp.Contains("1900/01/01"))
        //                            {
        //                                Valorx = "1900/01/01";
        //                            }

        //                            Valorx = "'" + Valorx + "'";
        //                            sentencia_bit = sentencia_bit + Campos2[i, 0] + " : " + Date + " ";
        //                        }
        //                        else
        //                        {
        //                            //LA FECHA NO TIENE UN FORMATO VALIDO
        //                            ///FALSE
        //                            //return false;
        //                            if (Util.IsNullOrEmpty(Valorx))//Si viene vacía
        //                            {
        //                                Valorx = "'1900/01/01'";
        //                            }
        //                            else
        //                            {
        //                                return false;
        //                            }

        //                        }

        //                        break;

        //                    default:

        //                        Console.WriteLine("DEBE DECLARAR BIEN LOS TIPOS DEL DATA TABLE, POR FAVOR HÁGALO Y VUELVA A INTENTARLO NUEVAMENTE");

        //                        return false;

        //                        break;



        //                }//fIN DEL SWITCH
        //                #endregion Conversiones


        //                if (i == longitud - 1)
        //                {

        //                    values = values + Valorx;//Si es el último dato no le concatenamos ,

        //                }
        //                else
        //                {
        //                    values = values + Valorx + ",";//Si no, si le concatenamos ,
        //                }


        //            }//FIN DEL FOR DE LLANADO DE VALUES



        //            if (cont_values == 800)//Inserta en paquetes de 800
        //            {
        //                values = values + ");";

        //                sentencia_sql = sentencia_sql + values;

        //                /*ESTE BLOQUE NOS DEVUELVE EL STRING DE CONEXION */
        //                try
        //                {
        //                    string mySqlSelectconex = "";
        //                    int johexitcant = 0;
        //                    int johexitbandera = 0;

        //                    //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
        //                    while (johexitbandera < 1 && johexitcant < 10)
        //                    {
        //                        Console.WriteLine("INTENTANDO CONEXIÓN AL SERVER DE BD: " + tipoconexion);

        //                        mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
        //                        if (!string.IsNullOrEmpty(mySqlSelectconex))
        //                        {
        //                            if (!mySqlSelectconex.Contains("error"))
        //                            {
        //                                Console.WriteLine("CONEXIÓN AL SERVER DE BD: " + tipoconexion + " EXITOSA");
        //                                johexitbandera = 1;
        //                            }
        //                            else
        //                            {
        //                                Console.WriteLine("CONEXIÓN AL SERVER DE BD: " + tipoconexion + " FALLIDA. REINTENTANDO...");
        //                                Thread.Sleep(1000);
        //                            }
        //                            //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
        //                        }
        //                        else
        //                        {
        //                            Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
        //                        }
        //                        johexitcant = johexitcant + 1;
        //                    }

        //                    if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex) && !string.IsNullOrEmpty(sentencia_sql))
        //                    {
        //                        //AQUI HACEMOS LA INSERCCION DE MYSQL
        //                        banderaTransaccion = conexion.execute_Mysql(sentencia_sql, mySqlSelectconex);
        //                    }
        //                    else
        //                    {
        //                        banderaTransaccion = false;

        //                        return banderaTransaccion;
        //                    }

        //                    fin = Linea;

        //                    inicio = Linea + 1;

        //                    sentencia_sql = sentencia_sql_encabezado;//Limpiamos la variable para comenzar a construir de nuevo el INSERT

        //                    cont_values = 1;//Reiniciamos el contador de values a 1

        //                }
        //                catch (Exception exc)
        //                {
        //                    fin = Linea;

        //                    string error = exc.ToString();
        //                    banderaTransaccion = false;

        //                    inicio = Linea + 1;

        //                    return false;//Si da error
        //                }
        //            }
        //            else
        //            {
        //                if (cont_values >= Dict_Insert_Masivo.Rows.Count || Linea == Dict_Insert_Masivo.Rows.Count)//Si el contador es mayor o igual a la cantidad de registros de la tabla
        //                {
        //                    values = values + ");";

        //                    sentencia_sql = sentencia_sql + values;

        //                    /*ESTE BLOQUE NOS DEVUELVE EL STRING DE CONEXION */
        //                    try
        //                    {
        //                        string mySqlSelectconex = "";
        //                        int johexitcant = 0;
        //                        int johexitbandera = 0;

        //                        //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
        //                        while (johexitbandera < 1 && johexitcant < 10)
        //                        {
        //                            Console.WriteLine("INTENTANDO CONEXIÓN AL SERVER DE BD: " + tipoconexion);

        //                            mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
        //                            if (!string.IsNullOrEmpty(mySqlSelectconex))
        //                            {
        //                                if (!mySqlSelectconex.Contains("error"))
        //                                {
        //                                    Console.WriteLine("CONEXIÓN AL SERVER DE BD: " + tipoconexion + " EXITOSA");
        //                                    johexitbandera = 1;
        //                                }
        //                                else
        //                                {
        //                                    Console.WriteLine("CONEXIÓN AL SERVER DE BD: " + tipoconexion + " FALLIDA. REINTENTANDO...");
        //                                    Thread.Sleep(1000);
        //                                }
        //                                //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
        //                            }
        //                            else
        //                            {
        //                                Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
        //                            }
        //                            johexitcant = johexitcant + 1;
        //                        }

        //                        if (!mySqlSelectconex.Contains("error") && !string.IsNullOrEmpty(mySqlSelectconex) && !string.IsNullOrEmpty(sentencia_sql))
        //                        {
        //                            //AQUI HACEMOS LA INSERCCION DE MYSQL
        //                            banderaTransaccion = conexion.execute_Mysql(sentencia_sql, mySqlSelectconex);
        //                        }
        //                        else
        //                        {
        //                            banderaTransaccion = false;




        //                            fin = Linea;


        //                            inicio = Linea + 1;

        //                            return false;
        //                        }


        //                        fin = Linea;



        //                        inicio = Linea + 1;

        //                        sentencia_sql = sentencia_sql_encabezado;//Limpiamos la variable para comenzar a construir de nuevo el INSERT

        //                        cont_values = 1;//Reiniciamos el contador de values a 1

        //                    }
        //                    catch (Exception exc)
        //                    {

        //                        string error = exc.ToString();
        //                        banderaTransaccion = false;

        //                        fin = Linea;

        //                        inicio = Linea + 1;


        //                        return false;//Si da error
        //                    }
        //                }
        //                else//Si no es el último valor de la tabla
        //                {
        //                    values = values + "),";//Concatenamos una coma para el próximo values
        //                    sentencia_sql = sentencia_sql + values;
        //                    cont_values++;//Autoincrementamos el contador de values
        //                }

        //            }


        //            Linea++;


        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        string error = e.ToString();
        //        banderaTransaccion = false;
        //    }


        //    return banderaTransaccion;
        //}

        //Nombre: Componente_sql
        //Creado por: Telecable(BJZ)
        //Descripción: Eliminación de tabla MYSQL
        //Fecha de creación: 06/09/2021
        //Fecha Modificación:
        //Cambios Realizados:

        //UTILIZAR CON PRECAUCIÓN!!!!!!!!!!!!!!!!
        //public bool MYSQLBORRADO(string tipoconexion, string tablaTruncate)
        //{
        //    bool banderaTransaccion = true;

        //    string mySqlSelectconex = "";
        //    int johexitcant = 0;
        //    int johexitbandera = 0;
        //    try
        //    {
        //        //Si la conexión no se establece reintentamos 9 veces esperando 2 segundos entre cada intento
        //        while (johexitbandera < 1 && johexitcant < 10)
        //        {
        //            Console.WriteLine("INTENTANDO CONEXIÓN AL SERVER DE BD: " + tipoconexion);

        //            mySqlSelectconex = conexion.MYSQLSTRINGCONNECT(tipoconexion);
        //            if (!string.IsNullOrEmpty(mySqlSelectconex))
        //            {
        //                if (!mySqlSelectconex.Contains("error"))
        //                {
        //                    johexitbandera = 1;
        //                }
        //                else
        //                {
        //                    Thread.Sleep(1000);
        //                }
        //                //Si no esta vacio y no dio error ponemos la bandera en 1 para salir del while
        //            }
        //            else
        //            {
        //                Thread.Sleep(1000); // Si no se trajo la conexion esperamos 2 segundos y volvemos a intentar
        //            }
        //            johexitcant = johexitcant + 1;
        //        }

        //        string xsql = "truncate table " + tablaTruncate;//Arma el query

        //        if (johexitbandera == 1)//Si pudo conectar con el servidor
        //        {
        //            banderaTransaccion = conexion.execute_Mysql(xsql, mySqlSelectconex);//Ejecuta el borrado de la tabla  
        //        }

        //    }
        //    catch (Exception exc)
        //    {

        //        string error = exc.ToString();
        //        banderaTransaccion = false;
        //    }


        //    return banderaTransaccion;
        //}

    }
}