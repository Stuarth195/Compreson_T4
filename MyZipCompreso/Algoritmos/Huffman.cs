using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyZipCompreso.Modelos;

namespace MyZipCompreso.Algoritmos
{
    public class ArbolHuffman
    {
        private List<NodoHuffman> nodos = new List<NodoHuffman>();
        public NodoHuffman Raiz { get; set; }
        public Dictionary<char, int> Frecuencias = new Dictionary<char, int>();

        public void Construir(string fuente)
        {
            Frecuencias.Clear();
            for (int i = 0; i < fuente.Length; i++)
            {
                if (!Frecuencias.ContainsKey(fuente[i]))
                {
                    Frecuencias.Add(fuente[i], 0);
                }

                Frecuencias[fuente[i]]++;
            }

            ConstruirDesdeFrecuencias(Frecuencias);
        }

        public void ConstruirDesdeFrecuencias(Dictionary<char, int> frecuencias)
        {
            Frecuencias = frecuencias;
            nodos.Clear();

            foreach (KeyValuePair<char, int> simbolo in Frecuencias)
            {
                nodos.Add(new NodoHuffman() { Simbolo = simbolo.Key, Frecuencia = simbolo.Value });
            }

            while (nodos.Count > 1)
            {
                List<NodoHuffman> nodosOrdenados = nodos.OrderBy(nodo => nodo.Frecuencia).ToList<NodoHuffman>();

                if (nodosOrdenados.Count >= 2)
                {
                    // Tomar los dos primeros elementos
                    List<NodoHuffman> tomados = nodosOrdenados.Take(2).ToList<NodoHuffman>();

                    // Crear un nodo padre combinando las frecuencias
                    NodoHuffman padre = new NodoHuffman()
                    {
                        Simbolo = '*',
                        Frecuencia = tomados[0].Frecuencia + tomados[1].Frecuencia,
                        Izquierdo = tomados[0],
                        Derecho = tomados[1]
                    };

                    nodos.Remove(tomados[0]);
                    nodos.Remove(tomados[1]);
                    nodos.Add(padre);
                }

                this.Raiz = nodos.FirstOrDefault();
            }
        }

        public BitArray Codificar(string fuente)
        {
            List<bool> fuenteCodificada = new List<bool>();

            for (int i = 0; i < fuente.Length; i++)
            {
                List<bool> simboloCodificado = this.Raiz.Recorrer(fuente[i], new List<bool>());
                if (simboloCodificado != null)
                {
                    fuenteCodificada.AddRange(simboloCodificado);
                }
            }

            BitArray bits = new BitArray(fuenteCodificada.ToArray());

            return bits;
        }

        public string Decodificar(BitArray bits)
        {
            NodoHuffman actual = this.Raiz;
            string decodificado = "";

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (actual.Derecho != null)
                    {
                        actual = actual.Derecho;
                    }
                }
                else
                {
                    if (actual.Izquierdo != null)
                    {
                        actual = actual.Izquierdo;
                    }
                }

                if (EsHoja(actual))
                {
                    decodificado += actual.Simbolo;
                    actual = this.Raiz;
                }
            }

            return decodificado;
        }

        public bool EsHoja(NodoHuffman nodo)
        {
            return (nodo.Izquierdo == null && nodo.Derecho == null);
        }
    }

    public class Huffman_C
    {
        public ArbolHuffman Arbol { get; private set; }

        public Huffman_C()
        {
            Arbol = new ArbolHuffman();
        }

        public void ComprimirADisco(string rutaEntrada, string rutaSalida)
        {
            string contenido = File.ReadAllText(rutaEntrada);
            Arbol.Construir(contenido);
            BitArray codificado = Arbol.Codificar(contenido);

            using (FileStream fs = new FileStream(rutaSalida, FileMode.Create))
            using (BinaryWriter escritor = new BinaryWriter(fs))
            {
                escritor.Write(Arbol.Frecuencias.Count);
                foreach (var item in Arbol.Frecuencias)
                {
                    escritor.Write(item.Key);
                    escritor.Write(item.Value);
                }
                escritor.Write(codificado.Length);
                byte[] bytes = new byte[(codificado.Length + 7) / 8];
                codificado.CopyTo(bytes, 0);
                escritor.Write(bytes);
            }
        }
    }

    public class Huffman_Dc
    {
        public void DescomprimirDesdeDisco(string rutaEntrada, string rutaSalida)
        {
            ArbolHuffman arbol = new ArbolHuffman();
            Dictionary<char, int> frecuencias = new Dictionary<char, int>();
            BitArray bits;

            using (FileStream fs = new FileStream(rutaEntrada, FileMode.Open))
            using (BinaryReader lector = new BinaryReader(fs))
            {
                int cantidad = lector.ReadInt32();
                for (int i = 0; i < cantidad; i++)
                {
                    char simbolo = lector.ReadChar();
                    int frecuencia = lector.ReadInt32();
                    frecuencias.Add(simbolo, frecuencia);
                }
                arbol.ConstruirDesdeFrecuencias(frecuencias);
                int cantidadBits = lector.ReadInt32();
                int cantidadBytes = (cantidadBits + 7) / 8;
                byte[] bytes = lector.ReadBytes(cantidadBytes);
                bits = new BitArray(bytes);
                BitArray bitsExactos = new BitArray(cantidadBits);
                for(int i = 0; i < cantidadBits; i++)
                {
                    bitsExactos[i] = bits[i];
                }
                bits = bitsExactos;
            }

            string decodificado = arbol.Decodificar(bits);
            File.WriteAllText(rutaSalida, decodificado);
        }
    }
}
