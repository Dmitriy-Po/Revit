using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Deijkstra
{
    // <summary>
    /// Ребро графа
    /// </summary>
    public class GraphEdge
    {
        /// <summary>
        /// Связанная вершина
        /// </summary>
        public GraphVertex ConnectedVertex { get; }

        /// <summary>
        /// Вес ребра
        /// </summary>
        public double EdgeWeight { get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="connectedVertex">Связанная вершина</param>
        /// <param name="weight">Вес ребра</param>
        public GraphEdge(GraphVertex connectedVertex, double weight)
        {
            ConnectedVertex = connectedVertex;
            EdgeWeight = weight;
        }
    }
}
