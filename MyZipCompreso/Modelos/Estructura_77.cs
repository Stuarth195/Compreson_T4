using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MyZipCompreso.Modelos
{
    // --------------------------------------------------------------------------------------------------------
    // 1. EL TOKEN
    // esta es la unidad de información en el algoritmo de LZ77
    // --------------------------------------------------------------------------------------------------------
    public class LZ77token
    {
        public int Desplazamiento { get; set; } // Veces que voy para atrás 
        public int Largo { get; set; } // Char que debo copiar 
        public char SiguienteChar { get; set; } // Siguiente char


        // Constructor del token 
        public LZ77token(int offset, int length, char nextChar)
        {
            Desplazamiento = offset;
            Largo = length;
            SiguienteChar = nextChar;
        }
    }

    // --------------------------------------------------------------------------------------------------------
    // 2. EL NODO
    // --------------------------------------------------------------------------------------------------------
    public class NodoLZ77
    {
        public LZ77token Datos { get; set; } // Valor que guarda el nodo
        public NodoLZ77 Siguiente { get; set; } // Siguiente nodo 

        public NodoLZ77(LZ77token datos)
        {
            Datos = datos;
            Siguiente = null;
        }
    }

    // --------------------------------------------------------------------------------------------------------
    // 3. LA LISTA ENLAZADA
    // --------------------------------------------------------------------------------------------------------
    public class ListaLZ77
    {
        private NodoLZ77 cabeza;
        private NodoLZ77 cola;

        public int Cantidad {  get; set; }

        // Constructor de la lista
        public ListaLZ77()
        {
            cabeza = null;
            cola = null;
            Cantidad = 0;
        }



        // Agrega un nuevo token al final de la lista
        public void Agregar (LZ77token token)
        {
            NodoLZ77 nuevoNodo = new NodoLZ77(token);

            if (cabeza == null)
            {
                cabeza = nuevoNodo;
                cola = nuevoNodo;
            }
            else
            {
                cola.Siguiente = nuevoNodo;
                cola = nuevoNodo;
            }
            Cantidad++;
        }



        // Obtener el token en una posición específica 
        public LZ77token Obtener (int indice)
        {
            if (indice < 0 || indice >= Cantidad)
            {
                throw new IndexOutOfRangeException("Índice fuera de rango en la Lista LZ77.");
            }

            NodoLZ77 actual = cabeza;
            // Avanzamos 'indice' veces
            for (int i = 0; i < indice; i++)
            {
                actual = actual.Siguiente;
            }

            return actual.Datos;
        }



        // Reinicia la lista
        public void Limpiar()
        {
            cabeza = null;
            cola = null;
            Cantidad = 0;
        }



        // Convierte la lista en un array para recorrerlo más rápido
        public LZ77token[] AArray()
        {
            LZ77token[] arreglo = new LZ77token[Cantidad];
            NodoLZ77 actual = cabeza;
            int i = 0;

            while (actual != null)
            {
                arreglo[i] = actual.Datos;
                actual = actual.Siguiente;
                i++;
            }
            return arreglo;
        }




    }




}
