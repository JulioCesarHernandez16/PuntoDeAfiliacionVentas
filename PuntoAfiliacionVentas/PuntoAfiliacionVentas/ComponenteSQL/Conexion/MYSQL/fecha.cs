using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL
{
    public class fecha
    {
        public string ValidacionFecha(string valor)
        {
            string Valorx = "";
            Console.WriteLine(valor);
            DateTime FECHA_FORMAT;
            string Date = valor;//Almacena la fecha
                                //Date = Util.LimpiaFecha(Date);//Limpiamos la fecha
            /*Fin validacion*/
            string[] formats = { 
                                //DIA/MES/ANNO
                                "dd/M/yyyy", "dd/M/yyyy HH:mm:ss", "d/M/yyyy", "d/M/yyyy h:mm:ss", "d/M/yyyy h:mm:ss tt", "dd/MM/yyyy hh:mm:ss",
                                "dd/MM/yyyy",
                                "d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt","d/M/yyyy hh:mm tt","d/M/yyyy hh tt",
                                "d/M/yyyy h:mm","d/M/yyyy h:mm","dd/MM/yyyy hh:mm", "dd/M/yyyy hh:mm",
                                //Nuevo
                                "dd/MM/yyyy hh:mm:ss tt",
                                "d/M/yyyy HH:mm:ss",
                                "dd/MM/yyyy HH:mm:ss",
                                //DIA-MES-ANNO
                                "dd-M-yyyy", "dd-M-yyyy HH:mm:ss", "d-M-yyyy", "d-M-yyyy h:mm:ss", "d-M-yyyy h:mm:ss tt", "dd-MM-yyyy hh:mm:ss",
                                "dd-MM-yyyy",
                                "d-M-yyyy h:mm:ss tt", "d-M-yyyy h:mm tt","d-M-yyyy hh:mm tt","d-M-yyyy hh tt",
                                "d-M-yyyy h:mm","d-M-yyyy h:mm","dd-MM-yyyy hh:mm", "dd-M-yyyy hh:mm",
                                //Nuevo
                                "dd-MM-yyyy hh:mm:ss tt",
                                "d-M-yyyy HH:mm:ss"
                                 };
            if (DateTime.TryParseExact(Date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
            {//si EXISTE UN FORMATO VALIDO 
                DateTime Salida = Convert.ToDateTime(FECHA_FORMAT);
                Valorx = "'" + Salida.ToString("yyyy-MM-dd") + "'";
                Console.WriteLine(Salida.ToString("yyyy-MM-dd"));
                return Valorx;
            }
            else
            {
                Date = Date.Trim();
                string[] formats2 = { 
                                    //DIA/MES/ANNO
                                    "yyyy-MM-dd", "yyyy/MM/dd",
                                     };
                if (DateTime.TryParseExact(Date, formats2, CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
                {
                    //*****************************************************************************
                    DateTime Salida = Convert.ToDateTime(FECHA_FORMAT);
                    Console.WriteLine(Salida.ToString("yyyy-MM-dd"));
                    Valorx = "'" + Salida.ToString("yyyy-MM-dd") + "'";
                    return Valorx;
                }
                else
                {

                    return Valorx;
                }
            }
        }

        public DateTime ConversionFecha(string valor)//
        {
            string Valorx = "";
            Console.WriteLine(valor);
            DateTime FECHA_FORMAT;
            string Date = valor;//Almacena la fecha
                                //Date = Util.LimpiaFecha(Date);//Limpiamos la fecha
            /*Fin validacion*/
            string[] formats = { 
                                //DIA/MES/ANNO
                                "dd/M/yyyy", "dd/M/yyyy HH:mm:ss", "d/M/yyyy", "d/M/yyyy h:mm:ss", "d/M/yyyy h:mm:ss tt", "dd/MM/yyyy hh:mm:ss",
                                "dd/MM/yyyy",
                                "d/M/yyyy h:mm:ss tt", "d/M/yyyy h:mm tt","d/M/yyyy hh:mm tt","d/M/yyyy hh tt",
                                "d/M/yyyy h:mm","d/M/yyyy h:mm","dd/MM/yyyy hh:mm", "dd/M/yyyy hh:mm",
                                //Nuevo
                                "dd/MM/yyyy hh:mm:ss tt",
                                "d/M/yyyy HH:mm:ss",
                                "dd/MM/yyyy HH:mm:ss",
                                //DIA-MES-ANNO
                                "dd-M-yyyy", "dd-M-yyyy HH:mm:ss", "d-M-yyyy", "d-M-yyyy h:mm:ss", "d-M-yyyy h:mm:ss tt", "dd-MM-yyyy hh:mm:ss",
                                "dd-MM-yyyy",
                                "d-M-yyyy h:mm:ss tt", "d-M-yyyy h:mm tt","d-M-yyyy hh:mm tt","d-M-yyyy hh tt",
                                "d-M-yyyy h:mm","d-M-yyyy h:mm","dd-MM-yyyy hh:mm", "dd-M-yyyy hh:mm",
                                //Nuevo
                                "dd-MM-yyyy hh:mm:ss tt",
                                "d-M-yyyy HH:mm:ss"
                                 };
            if (DateTime.TryParseExact(Date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
            {//si EXISTE UN FORMATO VALIDO                 
                return FECHA_FORMAT;
            }
            else
            {
                Date = Date.Trim();
                string[] formats2 = { 
                                    //DIA/MES/ANNO
                                    "yyyy-MM-dd", "yyyy/MM/dd",
                                     };
                if (DateTime.TryParseExact(Date, formats2, CultureInfo.InvariantCulture, DateTimeStyles.None, out FECHA_FORMAT))
                {
                    //*****************************************************************************
                    return FECHA_FORMAT;
                }
                else
                {

                    return FECHA_FORMAT;
                }
            }
        }
    }
}