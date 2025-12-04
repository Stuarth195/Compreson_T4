using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyZipCompreso.Modelos;

namespace MyZipCompreso.Algoritmos
{
   
    public class LZ77
    {
        private const int tamanoMax_VentanaDeslizante = 255; // Cuátos char atrás miro para buscar coinidencias
        private const int tamanoMax_BufferLectura = 255; // Cuántos char adelante puedo coincidir como max

        // ------------------------------------------------------------------------------------------------------------
        // Método de comprimir 
        // Toma un texto completo y lo convierte en una lista de token LZ77
        // ------------------------------------------------------------------------------------------------------------
        public ListaLZ77 Comprimir(string textoEntrante)
        {
            ListaLZ77 listaTokens = new ListaLZ77();
            int cursor = 0; // Posición actual en el texto que estamos procesamos


            // Mientras no lleguemos al final del texto
            while (cursor < textoEntrante.Length)
            {
                // Variables que guardan las mejores coincidencias encontrado
                int mejorDesplazamiento = 0;
                int mejorLargo = 0;

                int inicioVentana = Math.Max(0, cursor - tamanoMax_VentanaDeslizante);


                // Búsqueda de coincidencias
                for (int i = inicioVentana; i < cursor; i++)
                {
                    int longitudActual = 0;

                    while (cursor + longitudActual < textoEntrante.Length && // No nos salgamos del texto
                           longitudActual < tamanoMax_BufferLectura && // No superemos el límite de lectura
                           textoEntrante[i + longitudActual] == textoEntrante[cursor + longitudActual]) // Las caracteres sean iguales
                    {
                        longitudActual++;
                    }


                    // Si encontramos una coincidencia más larga que la anterior, entonces la guardamos
                    if (longitudActual > mejorLargo)
                    {
                        mejorLargo = longitudActual;
                        mejorDesplazamiento = cursor - i;
                    }
                }


                // Creación del token
                char siguienteCaracter;

                if (cursor + mejorLargo < textoEntrante.Length)
                {
                    siguienteCaracter = textoEntrante[cursor + mejorLargo];
                }
                else
                {
                    siguienteCaracter = '\0'; // Carácter nulo para indicar fin
                }

                LZ77token nuevoToken = new LZ77token(mejorDesplazamiento, mejorLargo, siguienteCaracter); // Se crea el token con los datos
                listaTokens.Agregar(nuevoToken); // Se agrega a la lista
                cursor += (mejorLargo + 1); // Saltamos los caracteres que ya comprimimos
            }

            return listaTokens;

        }



        // ------------------------------------------------------------------------------------------------------------
        // Método de descomprimir
        // ------------------------------------------------------------------------------------------------------------
        public string Descomprimir(ListaLZ77 tokens)
        {
            System.Text.StringBuilder salida = new System.Text.StringBuilder();
            LZ77token[] arrayTokens = tokens.AArray();

            foreach (LZ77token token in arrayTokens)
            {
                // Si hay desplazamiento y largo, copiamos del historial
                if (token.Largo > 0)
                {
                    int inicioCopia = salida.Length - token.Desplazamiento;

                    for (int i = 0; i < token.Largo; i++)
                    {
                        // Copiamos de "salida"
                        char caracterCopiado = salida[inicioCopia + i];
                        salida.Append(caracterCopiado);
                    }
                }

                // Agregamos carácter nuevo
                if (token.SiguienteChar != '\0')
                {
                    salida.Append(token.SiguienteChar);
                }

            }

            return salida.ToString();

        }



        // ------------------------------------------------------------------------------------------------------------
        // Método de serialización
        // Convertir tokens a string
        // ------------------------------------------------------------------------------------------------------------
        public string ConvertirListaAString(ListaLZ77 lista)
        {
            StringBuilder stringBuilder = new StringBuilder();
            LZ77token[] tokens = lista.AArray();

            foreach (var t in tokens)
            {
                string charStr = t.SiguienteChar.ToString();
                if (t.SiguienteChar == '\0')
                {
                    charStr = "NULL";
                }
                stringBuilder.Append($"{t.Desplazamiento},{t.Largo},{charStr}|");
            }
            return stringBuilder.ToString();
        }
    }

}