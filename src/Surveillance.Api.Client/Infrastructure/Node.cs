using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    public class Node<Z> : NodeBase where Z : class
    {
        private readonly Parent _parent;

        public Node(Parent parent)
        {
            _parent = parent;
        }

        public T Parent<T>() where T : Parent
        {
            return _parent as T;
        }

        protected Z AddField(string name)
        {
            _fields[name] = null;
            return this as Z;
        }

        protected T AddChild<T>(string name, T node) where T : Parent
        {
            _fields[name] = node;
            return node;
        }
    }
}
