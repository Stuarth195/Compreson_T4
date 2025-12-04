using MyZipCompreso.Algoritmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public class LZ78_Descompresor
    {
        private string texto_descomprimido_final = "";

        // ---------------- MÉTODOS PÚBLICOS ----------------

        public void Descomprimir(string ruta_archivo_origen)
        {
            string contenido_archivo = File.ReadAllText(ruta_archivo_origen);
            texto_descomprimido_final = Descomprimir_Aux(contenido_archivo);
        }

        public void GuardarTextoRecuperado(string ruta_archivo_destino)
        {
            GuardarTextoRecuperado_Aux(ruta_archivo_destino);
        }

        public void LimpiarMemoria()
        {
            LimpiarMemoria_Aux();
        }

        // ---------------- MÉTODOS PRIVADOS ----------------

        private string Descomprimir_Aux(string contenido_completo)
        {
            Dictionary<int, string> diccionario_conocimiento = new Dictionary<int, string>();
            string resultado_texto_plano = "";

            // Separar Cabecera de Cuerpo
            string[] partes_archivo = contenido_completo.Split(';');
            string parte_cabecera_base = partes_archivo[0].Replace("BASE:", "");
            string parte_cuerpo_codigos = partes_archivo[1];

            // 1. Reconstruir Diccionario Base
            for (int i = 0; i < parte_cabecera_base.Length; i++)
            {
                string letra_base = parte_cabecera_base[i].ToString();
                diccionario_conocimiento.Add(i + 1, letra_base);
            }

            // 2. Procesar Códigos (Flujo Natural LZ78)
            // Quitamos el último '|' vacío si existe
            string[] lista_pares_codigos = parte_cuerpo_codigos.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string par_codigo in lista_pares_codigos)
            {
                string[] datos_par = par_codigo.Split(',');
                int indice_prefijo = int.Parse(datos_par[0]);
                string caracter_nuevo = datos_par[1];

                if (caracter_nuevo == "NULL")
                {
                    caracter_nuevo = "";
                }

                string cadena_prefijo_recuperada = "";

                if (indice_prefijo != 0)
                {
                    if (diccionario_conocimiento.ContainsKey(indice_prefijo))
                    {
                        cadena_prefijo_recuperada = diccionario_conocimiento[indice_prefijo];
                    }
                }

                string cadena_reconstruida = cadena_prefijo_recuperada + caracter_nuevo;

                // A. Agregamos al texto final
                resultado_texto_plano += cadena_reconstruida;

                // B. Aprendemos la nueva palabra (Actualizar Diccionario)
                int nuevo_id_diccionario = diccionario_conocimiento.Count + 1;
                diccionario_conocimiento.Add(nuevo_id_diccionario, cadena_reconstruida);
            }

            return resultado_texto_plano;
        }

        private void GuardarTextoRecuperado_Aux(string ruta_destino)
        {
            if (texto_descomprimido_final != "")
            {
                File.WriteAllText(ruta_destino, texto_descomprimido_final);
            }
        }

        private void LimpiarMemoria_Aux()
        {
            texto_descomprimido_final = "";
        }
    }
}




