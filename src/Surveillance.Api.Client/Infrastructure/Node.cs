using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Surveillance.Api.Client.Infrastructure
{
    public class Node<Z> : NodeParent where Z : class
    {
        private readonly NodeParent _parent;

        public Node(NodeParent parent)
        {
            _parent = parent;
        }

        public T Parent<T>() where T : NodeParent
        {
            return _parent as T;
        }

        protected Z AddField(string name)
        {
            _fields[name] = null;
            return this as Z;
        }

        protected T AddChild<T>(string name, T node) where T : NodeParent
        {
            _fields[name] = node;
            return node;
        }
    }
}
