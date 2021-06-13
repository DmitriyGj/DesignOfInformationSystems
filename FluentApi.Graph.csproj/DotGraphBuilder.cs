using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentApi.Graph
{
    public interface IAttributessBuilder<out T>
    {
        T Color(string color);
        T FontSize(int fontSize);
        T Label(string label);
    }
        
    public interface IBuilder
    {
        GraphNodeBuilder AddNode(string nodeName);
        GraphEdgeBuilder AddEdge(string sourceNode,string endNode);
        string Build();
    }

    public class DotGraphBuilder :IBuilder
    {
        private readonly Graph graph;
        private DotGraphBuilder(string graphName, bool directed)
        {
            graph = new Graph(graphName, directed, false);
        }
        
        public static IBuilder DirectedGraph(string graphName)=>new DotGraphBuilder(graphName,true);
        public static IBuilder UndirectedGraph(string graphName) => new DotGraphBuilder(graphName, false);
        public string Build()=> graph.ToDotFormat();
        public GraphNodeBuilder AddNode(string nodeName)
        {
            graph.AddNode(nodeName);
            return new GraphNodeBuilder(this,graph.Nodes.Last());
        }
        
        public GraphEdgeBuilder AddEdge(string sourceNode, string endNode)
        {
            graph.AddEdge(sourceNode, endNode);
            return new GraphEdgeBuilder(this,graph.Edges.Last());
        }
    }
    
    public class GraphNodeBuilder: IBuilder
    {
        private readonly DotGraphBuilder parent;
        private readonly GraphNode node;

        public GraphNodeBuilder( DotGraphBuilder parent, GraphNode node)
        {
            this.parent = parent;
            this.node = node;
        }
        
        public GraphNodeBuilder AddNode(string nodeName) => parent.AddNode(nodeName);
        public GraphEdgeBuilder AddEdge(string sourceNode, string endNode) => parent.AddEdge(sourceNode, endNode);
        public string Build()=> parent.Build();

        public IBuilder With(Action<AttributessOfNodeBuilder> attributes)
        {
            var parameters = new AttributessOfNodeBuilder();
            attributes(parameters);
            foreach (var param in parameters.Attributes)
                node.Attributes.Add(param.Key,param.Value);
            return parent;
        }
    }

    public class GraphEdgeBuilder : IBuilder
    {
        private readonly DotGraphBuilder parent;
        private readonly GraphEdge edge;
        public GraphEdgeBuilder(DotGraphBuilder parent,GraphEdge edge)
        {
            this.parent = parent;
            this.edge = edge;
        }
        
        public GraphNodeBuilder AddNode(string nodeName) =>parent.AddNode(nodeName);
        public GraphEdgeBuilder AddEdge(string sourceNode, string endNode) => parent.AddEdge(sourceNode,endNode);
        public string Build() => parent.Build();
        public IBuilder With(Action<AttributessOfEdgeBuilder> attributes)
        {
            var parameters = new AttributessOfEdgeBuilder();
            attributes(parameters);
            foreach (var val in parameters.Attributes)
                edge.Attributes.Add(val.Key,val.Value);
            return parent;
        }
    }

    public enum NodeShape
    {
        Box,Ellipse
    }
    
    public class AttributessOfNodeBuilder: IAttributessBuilder<AttributessOfNodeBuilder>
    {
        internal readonly Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public AttributessOfNodeBuilder Color(string color)
        {
            Attributes["color"] = color;
            return this;
        }
        
        public AttributessOfNodeBuilder FontSize(int fontSize)
        {
            Attributes["fontsize"] = fontSize.ToString();
            return this;
        }
        
        public AttributessOfNodeBuilder Shape(NodeShape shape)
        {
            Attributes["shape"] = shape.ToString().ToLower();
            return this;
        }
        
        public AttributessOfNodeBuilder Label(string label)
        {
            Attributes["label"] = label;
            return this;
        }
    }

    public class AttributessOfEdgeBuilder:IAttributessBuilder<AttributessOfEdgeBuilder>
    {
        internal readonly Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public AttributessOfEdgeBuilder Color(string color)
        {
            Attributes["color"] = color;
            return this;
        }
        
        public AttributessOfEdgeBuilder FontSize(int fontSize)
        {
            Attributes["fontsize"] = fontSize.ToString();
            return this;
        }
        
        public AttributessOfEdgeBuilder Weight(double weight)
        {
            Attributes["weight"] = weight.ToString();
            return this;
        }
        
        public AttributessOfEdgeBuilder Label(string label)
        {
            Attributes["label"] = label;
            return this;
        }
    }
}