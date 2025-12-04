using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MyZipCompreso.Algoritmos
{
    public class LZ78_Compresor
    {
        private string contenido_comprimido_para_guardar = "";

        // ---------------- MÉTODOS PÚBLICOS ----------------

        public void Comprimir(string texto_para_comprimir)
        {
            contenido_comprimido_para_guardar = Comprimir_Aux(texto_para_comprimir);
        }

        public void GuardarArchivo(string ruta_archivo_destino)
        {
            GuardarArchivo_Aux(ruta_archivo_destino);
        }

        public void LimpiarMemoria()
        {
            LimpiarMemoria_Aux();
        }

        // ---------------- MÉTODOS PRIVADOS ----------------

        private string Comprimir_Aux(string texto_entrada)
        {
            string cadena_resultado_formateada = "";
            Dictionary<string, int> diccionario_cadenas_existentes = new Dictionary<string, int>();

            // 1. Cabecera (Alfabeto Base)
            cadena_resultado_formateada += "BASE:";
            List<char> caracteres_unicos = texto_entrada.Distinct().ToList();

            for (int i = 0; i < caracteres_unicos.Count; i++)
            {
                string letra_unica = caracteres_unicos[i].ToString();
                cadena_resultado_formateada += letra_unica;

                // Inicializamos diccionario con índices base (1, 2, 3...)
                diccionario_cadenas_existentes.Add(letra_unica, i + 1);
            }
            cadena_resultado_formateada += ";";

            // 2. Cuerpo (Compresión LZ78)
            string cadena_acumulada_actual = "";
            int contador_indices_diccionario = caracteres_unicos.Count;

            foreach (char caracter_actual in texto_entrada)
            {
                string posible_nueva_cadena = cadena_acumulada_actual + caracter_actual;

                if (diccionario_cadenas_existentes.ContainsKey(posible_nueva_cadena))
                {
                    cadena_acumulada_actual = posible_nueva_cadena;
                }
                else
                {
                    int indice_prefijo_conocido = 0;
                    if (cadena_acumulada_actual != "")
                    {
                        indice_prefijo_conocido = diccionario_cadenas_existentes[cadena_acumulada_actual];
                    }

                    cadena_resultado_formateada += indice_prefijo_conocido + "," + caracter_actual + "|";

                    contador_indices_diccionario++;
                    diccionario_cadenas_existentes.Add(posible_nueva_cadena, contador_indices_diccionario);

                    cadena_acumulada_actual = "";
                }
            }

            if (cadena_acumulada_actual != "")
            {
                int indice_final = diccionario_cadenas_existentes[cadena_acumulada_actual];
                cadena_resultado_formateada += indice_final + ",NULL|";
            }

            return cadena_resultado_formateada;
        }

        private void GuardarArchivo_Aux(string ruta_archivo)
        {
            if (contenido_comprimido_para_guardar != "")
            {
                File.WriteAllText(ruta_archivo, contenido_comprimido_para_guardar);
            }
        }

        private void LimpiarMemoria_Aux()
        {
            contenido_comprimido_para_guardar = "";
        }
    }
}