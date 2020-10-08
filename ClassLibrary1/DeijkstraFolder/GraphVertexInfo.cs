using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Deijkstra
{
    /// <summary>
    /// Информация о вершине
    /// </summary>
    public class GraphVertexInfo
    {
        /// <summary>
        /// Вершина
        /// </summary>
        public GraphVertex Vertex { get; set; }

        /// <summary>
        /// Не посещенная вершина
        /// </summary>
        public bool IsUnvisited { get; set; }

        /// <summary>
        /// Сумма весов ребер
        /// </summary>
        public double EdgesWeightSum { get; set; }

        /// <summary>
        /// Предыдущая вершина
        /// </summary>
        public GraphVertex PreviousVertex { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="vertex">Вершина</param>
        public GraphVertexInfo(GraphVertex vertex)
        {
            Vertex = vertex;
            IsUnvisited = true;
            EdgesWeightSum = int.MaxValue;
            PreviousVertex = null;
        }
    }
}
