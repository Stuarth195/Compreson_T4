using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyZipCompreso.Modelos;

namespace MyZipCompreso.Algoritmos
{
    public class Nodo_Arbol_Huffman
    {
        public char Caracter { get; set; }
        public int Frecuencia { get; set; }
        public Nodo_Arbol_Huffman Izquierda { get; set; }
        public Nodo_Arbol_Huffman Derecha { get; set; }

        public bool EsHoja()
        {
            return (Izquierda == null && Derecha == null);
        }
    }

    public class Huffman_Compresor
    {
        private string resultado_comprimido_final = "";
        private Dictionary<char, string> diccionario_codigos = new Dictionary<char, string>();
        public void Comprimir(string texto_original)
        {
            if (string.IsNullOrEmpty(texto_original))
            {
                resultado_comprimido_final = "";
                return;
            }
            Dictionary<char, int> tabla_frecuencias = ObtenerFrecuencias(texto_original);
            Nodo_Arbol_Huffman raiz_arbol = ConstruirArbol(tabla_frecuencias);
            diccionario_codigos.Clear();
            GenerarCodigosParaCadaLetra(raiz_arbol, "");
            StringBuilder cadena_bits = new StringBuilder();
            foreach (char letra in texto_original)
            {
                cadena_bits.Append(diccionario_codigos[letra]);
            }
            resultado_comprimido_final = FormatearSalida(tabla_frecuencias, cadena_bits.ToString());
        }

        public string ObtenerResultadoString()
        {
            return resultado_comprimido_final;
        }

        //MÉTODOS PRIVADOS

        private Dictionary<char, int> ObtenerFrecuencias(string texto)
        {
            Dictionary<char, int> conteo = new Dictionary<char, int>();
            foreach (char c in texto)
            {
                if (conteo.ContainsKey(c))
                {
                    conteo[c]++;
                }
                else
                {
                    conteo.Add(c, 1);
                }
            }
            return conteo;
        }

        private Nodo_Arbol_Huffman ConstruirArbol(Dictionary<char, int> frecuencias)
        {
            List<Nodo_Arbol_Huffman> lista_nodos = new List<Nodo_Arbol_Huffman>();

            foreach (var item in frecuencias)
            {
                lista_nodos.Add(new Nodo_Arbol_Huffman() { Caracter = item.Key, Frecuencia = item.Value });
            }
            while (lista_nodos.Count > 1)
            {
                lista_nodos = lista_nodos.OrderBy(n => n.Frecuencia).ToList();
                Nodo_Arbol_Huffman izquierdo = lista_nodos[0];
                Nodo_Arbol_Huffman derecho = lista_nodos[1];
                Nodo_Arbol_Huffman padre = new Nodo_Arbol_Huffman()
                {
                    Caracter = '*',
                    Frecuencia = izquierdo.Frecuencia + derecho.Frecuencia,
                    Izquierda = izquierdo,
                    Derecha = derecho
                };
                lista_nodos.Remove(izquierdo);
                lista_nodos.Remove(derecho);
                lista_nodos.Add(padre);
            }
            return lista_nodos.FirstOrDefault();
        }

        private void GenerarCodigosParaCadaLetra(Nodo_Arbol_Huffman nodo_actual, string codigo_actual)
        {
            if (nodo_actual == null) return;
            if (nodo_actual.EsHoja())
            {
                diccionario_codigos.Add(nodo_actual.Caracter, codigo_actual);
                return;
            }
            GenerarCodigosParaCadaLetra(nodo_actual.Izquierda, codigo_actual + "0");
            GenerarCodigosParaCadaLetra(nodo_actual.Derecha, codigo_actual + "1");
        }

        private string FormatearSalida(Dictionary<char, int> frecuencias, string bits)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TABLA:");
            foreach (var item in frecuencias)
            {
                sb.Append((int)item.Key + "-" + item.Value + ",");
            }
            if (sb.Length > 0 && sb[sb.Length - 1] == ',')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append(";BITS:");
            sb.Append(bits);
            return sb.ToString();
        }
    }

    public class Huffman_Descompresor
    {
        private string texto_recuperado = "";

        //MÉTODOS PÚBLICOS

        public void DescomprimirDesdeCadena(string contenido_comprimido)
        {
            if (string.IsNullOrEmpty(contenido_comprimido))
            {
                texto_recuperado = "";
                return;
            }
            string[] partes_principales = contenido_comprimido.Split(new string[] { ";BITS:" }, StringSplitOptions.None);
            
            if (partes_principales.Length < 2)
            {
                texto_recuperado = "ERROR_FORMATO";
                return;
            }

            string parte_tabla = partes_principales[0].Replace("TABLA:", "");
            string parte_bits = partes_principales[1];
            Dictionary<char, int> tabla_frecuencias = ReconstruirTabla(parte_tabla);
            Nodo_Arbol_Huffman raiz = ConstruirArbol(tabla_frecuencias);
            texto_recuperado = DecodificarBits(raiz, parte_bits);
        }

        public string ObtenerResultadoString()
        {
            return texto_recuperado;
        }

        private Dictionary<char, int> ReconstruirTabla(string cadena_tabla)
        {
            Dictionary<char, int> frecuencias = new Dictionary<char, int>();
            string[] pares = cadena_tabla.Split(',');

            foreach (string par in pares)
            {
                if (string.IsNullOrEmpty(par)) continue;

                string[] datos = par.Split('-');
                if (datos.Length == 2)
                {
                    int valor_char = int.Parse(datos[0]);
                    int valor_frec = int.Parse(datos[1]);
                    
                    frecuencias.Add((char)valor_char, valor_frec);
                }
            }

            return frecuencias;
        }

        // Este método es idéntico al del compresor, es para tener el mismo árbol
        private Nodo_Arbol_Huffman ConstruirArbol(Dictionary<char, int> frecuencias)
        {
            List<Nodo_Arbol_Huffman> lista_nodos = new List<Nodo_Arbol_Huffman>();

            foreach (var item in frecuencias)
            {
                lista_nodos.Add(new Nodo_Arbol_Huffman() { Caracter = item.Key, Frecuencia = item.Value });
            }

            while (lista_nodos.Count > 1)
            {
                lista_nodos = lista_nodos.OrderBy(n => n.Frecuencia).ToList();

                Nodo_Arbol_Huffman izquierdo = lista_nodos[0];
                Nodo_Arbol_Huffman derecho = lista_nodos[1];

                Nodo_Arbol_Huffman padre = new Nodo_Arbol_Huffman()
                {
                    Caracter = '*',
                    Frecuencia = izquierdo.Frecuencia + derecho.Frecuencia,
                    Izquierda = izquierdo,
                    Derecha = derecho
                };

                lista_nodos.Remove(izquierdo);
                lista_nodos.Remove(derecho);
                lista_nodos.Add(padre);
            }

            return lista_nodos.FirstOrDefault();
        }

        private string DecodificarBits(Nodo_Arbol_Huffman raiz, string bits)
        {
            StringBuilder resultado = new StringBuilder();
            Nodo_Arbol_Huffman nodo_actual = raiz;

            foreach (char bit in bits)
            {
                if (bit == '0')
                {
                    nodo_actual = nodo_actual.Izquierda;
                }
                else if (bit == '1')
                {
                    nodo_actual = nodo_actual.Derecha;
                }
                if (nodo_actual.EsHoja())
                {
                    resultado.Append(nodo_actual.Caracter);
                    nodo_actual = raiz;
                }
            }

            return resultado.ToString();
        }
    }
}
