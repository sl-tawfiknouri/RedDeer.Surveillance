namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    public class Node<Z> : NodeBase
        where Z : class
    {
        private readonly Parent _parent;

        public Node(Parent parent)
        {
            this._parent = parent;
        }

        public T Parent<T>()
            where T : Parent
        {
            return this._parent as T;
        }

        protected T AddChild<T>(string name, T node)
            where T : Parent
        {
            this._fields[name] = node;
            return node;
        }

        protected Z AddField(string name)
        {
            this._fields[name] = null;
            return this as Z;
        }
    }
}