using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyZipCompreso.Modelos
{
    public class NodoHuffman
    {
        public char Simbolo { get; set; }
        public int Frecuencia { get; set; }
        public NodoHuffman Derecho { get; set; }
        public NodoHuffman Izquierdo { get; set; }

        public List<bool> Recorrer(char simbolo, List<bool> datos)
        {
            // Hoja
            if (Derecho == null && Izquierdo == null)
            {
                if (simbolo.Equals(this.Simbolo))
                {
                    return datos;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> izquierda = null;
                List<bool> derecha = null;

                if (Izquierdo != null)
                {
                    List<bool> caminoIzquierdo = new List<bool>();
                    caminoIzquierdo.AddRange(datos);
                    caminoIzquierdo.Add(false);

                    izquierda = Izquierdo.Recorrer(simbolo, caminoIzquierdo);
                }

                if (Derecho != null)
                {
                    List<bool> caminoDerecho = new List<bool>();
                    caminoDerecho.AddRange(datos);
                    caminoDerecho.Add(true);
                    derecha = Derecho.Recorrer(simbolo, caminoDerecho);
                }

                if (izquierda != null)
                {
                    return izquierda;
                }
                else
                {
                    return derecha;
                }
            }
        }
    }
}