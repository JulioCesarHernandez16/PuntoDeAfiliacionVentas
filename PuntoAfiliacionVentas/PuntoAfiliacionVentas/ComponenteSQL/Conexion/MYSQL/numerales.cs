using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PuntoAfiliacionVentas.ComponenteSQL.Conexion.MYSQL
{
    public class numerales
    {
        public string ValidacionDouble(string valor)
        {
            Console.WriteLine("DOUBLE ORIGINAL:" + valor);
            if (string.IsNullOrEmpty(valor))
            {
                valor = "0";
                return valor;
            }
            //****************************************************************************************************
            valor = valor.Replace(",", ".");
            Console.WriteLine("DOUBLE NUEVO:" + valor);
            string Valorx = "";
            NumberStyles style;
            double number = 0;
            style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            CultureInfo culture = CultureInfo.InvariantCulture;//Seteamos la cultura para trabajar con "."
            if (Double.TryParse(valor, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
            {//Correcto es numerico
             //numerico = true;// Bandera para no concantenar el valor a un string 
                /*Previene que se cambie el separador*/
                /**************************************************************************************************/
                Console.WriteLine("DOUBLE NUMBER:" + number);
                Valorx = number.ToString().Replace(",", ".");
                Console.WriteLine("DOUBLE Valorx:" + Valorx);
            }
            else
            {
                if (valor.Contains(",") && valor.Contains("."))//Si el número dio error y contiene esos dos simbolos(, y .)
                {
                    string Temp_Numero = valor;
                    //11,255.01
                    //11.255,01
                    Temp_Numero = Temp_Numero.Replace(",", "%");//Reemplazamos las , por un simbolo temporal
                    Temp_Numero = Temp_Numero.Replace(".", "");//Reemplazamos los . por un simbolo temporal
                    /************************************LIMPIAMOS EL NÚMERO******************************************/
                    Temp_Numero = Temp_Numero.Replace("%", ".");//Reemplazamos el simbolo temporal por el punto %
                                                                //Temp_Numero = Temp_Numero.Replace("$", ",");//Reemplazamos el simbolo temporal por la coma $
                    /**************************REINTENTAMOS LA CONVERSIÓN**********************************/
                    if (Double.TryParse(Temp_Numero, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                    {//Correcto es numerico
                     //numerico = true;// Bandera para no concantenar el valor a un string 
                        Console.WriteLine("DOUBLE NUMBER:" + number);
                        Valorx = number.ToString().Replace(",", ".");
                        Console.WriteLine("DOUBLE Valorx:" + Valorx);
                        return Valorx;
                    }
                }
            }
            return Valorx;
        }

        public string ValidacionInt32(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                valor = "0";
                return valor;
            }
            //****************************************************************************************************
            valor = valor.Replace(",", ".");
            //****************************************************************************************************
            Console.WriteLine("INT32:" + valor);
            string Valorx = "";
            NumberStyles style;
            int number = 0;
            style = NumberStyles.AllowThousands;
            CultureInfo culture = CultureInfo.InvariantCulture;//Seteamos la cultura para trabajar con "."
            if (Int32.TryParse(valor, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
            {
                /**************************************************************************************************/
                Valorx = number.ToString().Replace(",", ".");
            }
            return Valorx;
        }

        public string ValidacionInt64(string valor)
        {
            if (string.IsNullOrEmpty(valor))
            {
                valor = "0";
                return valor;
            }
            //****************************************************************************************************
            valor = valor.Replace(",", ".");
            //****************************************************************************************************
            Console.WriteLine("INT64:" + valor);
            string Valorx = "";
            NumberStyles style;
            long number = 0;
            style = NumberStyles.AllowThousands;
            CultureInfo culture = CultureInfo.InvariantCulture;//Seteamos la cultura para trabajar con "."
            if (Int64.TryParse(valor, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
            {
                /**************************************************************************************************/
                Valorx = number.ToString().Replace(",", ".");
            }
            return Valorx;
        }

        public string ValidacionDecimal(string valor)
        {
            Console.WriteLine("DECIMAL ORIGINAL:" + valor);
            if (string.IsNullOrEmpty(valor))
            {
                valor = "0";
                return valor;
            }

            //****************************************************************************************************
            valor = valor.Replace(",", ".");
            //****************************************************************************************************
            Console.WriteLine("DECIMAL NUEVO:" + valor);

            string Valorx = "";
            NumberStyles style;
            decimal number = 0;
            style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            CultureInfo culture = CultureInfo.InvariantCulture;//Seteamos la cultura para trabajar con "."
            if (Decimal.TryParse(valor, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
            {//Correcto es numerico
             //numerico = true;// Bandera para no concantenar el valor a un string 
                /*Previene que se cambie el separador*/
                /**************************************************************************************************/
                Console.WriteLine("DECIMAL NUMBER:" + number);
                Valorx = number.ToString().Replace(",", ".");
                Console.WriteLine("DECIMAL Valorx:" + Valorx);
            }
            else
            {
                if (valor.Contains(",") && valor.Contains("."))//Si el número dio error y contiene esos dos simbolos(, y .)
                {
                    string Temp_Numero = valor;
                    //11,255.01
                    //11.255,01
                    Temp_Numero = Temp_Numero.Replace(",", "%");//Reemplazamos las , por un simbolo temporal
                    Temp_Numero = Temp_Numero.Replace(".", "");//Reemplazamos los . por un simbolo temporal
                    /************************************LIMPIAMOS EL NÚMERO******************************************/
                    Temp_Numero = Temp_Numero.Replace("%", ".");//Reemplazamos el simbolo temporal por el punto %
                                                                //Temp_Numero = Temp_Numero.Replace("$", ",");//Reemplazamos el simbolo temporal por la coma $
                    /**************************REINTENTAMOS LA CONVERSIÓN**********************************/
                    if (Decimal.TryParse(Temp_Numero, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                    {
                        Console.WriteLine("DECIMAL NUMBER:" + number);
                        Valorx = number.ToString().Replace(",", ".");
                        Console.WriteLine("DECIMAL Valorx:" + Valorx);
                        //Valorx = Valorx.Replace(",", ".");
                        return Valorx;
                    }
                }
            }
            return Valorx;
        }

        public string ValidacionFloat(string valor)
        {
            Console.WriteLine("FLOAT ORIGINAL:" + valor);
            if (string.IsNullOrEmpty(valor))
            {
                valor = "0";
                return valor;
            }

            //****************************************************************************************************
            valor = valor.Replace(",", ".");
            //****************************************************************************************************
            Console.WriteLine("FLOAT NUEVO:" + valor);
            string Valorx = "";
            NumberStyles style;
            float number = 0;
            style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            CultureInfo culture = CultureInfo.InvariantCulture;//Seteamos la cultura para trabajar con "."
            if (float.TryParse(valor, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
            {//Correcto es numerico
             //numerico = true;// Bandera para no concantenar el valor a un string 
                /*Previene que se cambie el separador*/
                /**************************************************************************************************/
                Console.WriteLine("FLOAT NUMBER:" + number);
                Valorx = number.ToString().Replace(",", ".");
                Console.WriteLine("DECIMAL Valorx:" + valor);
            }
            else
            {
                if (valor.Contains(",") && valor.Contains("."))//Si el número dio error y contiene esos dos simbolos(, y .)
                {
                    string Temp_Numero = valor;
                    //11,255.01
                    //11.255,01
                    Temp_Numero = Temp_Numero.Replace(",", "%");//Reemplazamos las , por un simbolo temporal
                    Temp_Numero = Temp_Numero.Replace(".", "");//Reemplazamos los . por un simbolo temporal
                    /************************************LIMPIAMOS EL NÚMERO******************************************/
                    Temp_Numero = Temp_Numero.Replace("%", ".");//Reemplazamos el simbolo temporal por el punto %
                                                                //Temp_Numero = Temp_Numero.Replace("$", ",");//Reemplazamos el simbolo temporal por la coma $
                    /**************************REINTENTAMOS LA CONVERSIÓN**********************************/
                    if (float.TryParse(Temp_Numero, style, culture, out number))//Si es TRUE EL FORMATO DEL NUMERO ES VALIDO
                    {
                        Console.WriteLine("FLOAT NUMBER:" + number);
                        Valorx = number.ToString().Replace(",", ".");
                        Console.WriteLine("DECIMAL Valorx:" + Valorx);
                        return Valorx;


                    }
                }
            }
            return Valorx;
        }
    }
}