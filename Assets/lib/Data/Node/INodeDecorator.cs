namespace Assets.lib.Data.Node
{
	partial class NodeGraph
	{
		public interface INodeDecorator
		{
			Node DecoratedNode { get; set; }
		}
	}
}