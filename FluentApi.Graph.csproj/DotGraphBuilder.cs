using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework.Constraints;

namespace FluentApi.Graph
{
    public abstract class GraphPartBuilder
    {
        protected DotGraphBuilder parent;

        protected GraphPartBuilder(DotGraphBuilder parent)
        {
            this.parent = parent;
        }

        public GraphPartBuilder AddNode(string nodeName)
          =>parent.AddNode(nodeName);

        public GraphPartBuilder AddEdge(string edgeStart, string edgeEnd)
        => parent.AddEdge(edgeStart, edgeEnd);

        public String Build()=>parent.Build();

        public abstract GraphPartBuilder With(Action<AttributesBuilder> applier);
    }

    public class DotGraphBuilder 
    {
        readonly Graph graph;
        private DotGraphBuilder(string graphName, bool directed)
        {
            graph = new Graph(graphName, directed, false);
        }
        
        public static DotGraphBuilder DirectedGraph(string graphName)=>new DotGraphBuilder(graphName,true);
        public static DotGraphBuilder UndirectedGraph(string graphName) => new DotGraphBuilder(graphName, false);
        public string Build()=> graph.ToDotFormat();
        public GraphPartBuilder AddNode(string nodeName)
        {
            graph.AddNode(nodeName);
            return new NodeBuilder(this,graph.Nodes.Last());
        }
        
        public GraphPartBuilder AddEdge(string sourceNode, string endNode)
        {
            graph.AddEdge(sourceNode, endNode);
            return new EdgeBuilder(this,graph.Edges.Last());
        }
    }
    
    public class NodeBuilder:GraphPartBuilder
    {
        private readonly GraphNode node;

        public NodeBuilder( DotGraphBuilder parent, GraphNode node):base(parent)
        {
            this.node = node;
        }

        public override GraphPartBuilder With(Action<AttributesBuilder> applier)
        {
            applier(new AttributesOfNodeBuilder(node));
            return this;
        }
    }

    public class EdgeBuilder : GraphPartBuilder
    {
        private readonly GraphEdge edge;
        public EdgeBuilder(DotGraphBuilder parent,GraphEdge edge):base(parent)
        {
            this.edge = edge;
        }

        public override GraphPartBuilder With(Action<AttributesBuilder> applier)
        {
            applier(new AttributesOfEdgeBuilder(edge));
            return this;
        }
    }

    public enum NodeShape
    {
        Box,Ellipse
    }

    public abstract class AttributesBuilder
    {
        internal Dictionary<string, string> Attributes;
        protected AttributesBuilder (Dictionary<string, string> dict)
        {
            Attributes = dict;
        }
            
        public AttributesBuilder Color(string color)
        {
            Attributes["color"] = color;
            return this; 
        }

        public AttributesBuilder FontSize(int fontSize)
        {
            Attributes["fontsize"] = fontSize.ToString();
            return this;
        }

        public AttributesBuilder Label(string label)
        {
            Attributes["label"] = label;
            return this;
        }

        public virtual AttributesOfNodeBuilder Shape(NodeShape shape)
        {
            throw new NotImplementedException();
        }

        public virtual AttributesOfEdgeBuilder Weight(double weihgt)
        {
            throw new NotImplementedException();
        }
    }
    
    public class AttributesOfNodeBuilder:AttributesBuilder
    {
        public override AttributesOfNodeBuilder Shape(NodeShape shape)
        {
            Attributes["shape"] = shape.ToString().ToLower();
            return this;
        }

        public AttributesOfNodeBuilder(GraphNode graphNode) : base(graphNode.Attributes)
        {
        }
    }

    public class AttributesOfEdgeBuilder: AttributesBuilder
    {
        public override AttributesOfEdgeBuilder Weight(double weight)
        {
            Attributes["weight"] = weight.ToString(CultureInfo.InvariantCulture);
            return this;
        }

        public AttributesOfEdgeBuilder(GraphEdge graphEdge) : base(graphEdge.Attributes)
        {
        }
    }
}