using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luxcinder.Functions.UISystem.UICore;
public class LayoutSolver
{
	// 所有需要求解的节点
	private readonly List<LuxcinderUIBase> _nodes = new();

	// 添加根节点（将包含所有子节点）
	public void AddRoot(LuxcinderUIBase root)
	{
		_nodes.Add(root);
		CollectNodes(root);
	}

	// 收集所有子节点
	private void CollectNodes(LuxcinderUIBase node)
	{
		foreach (var child in node.Children)
		{
			_nodes.Add(child);
			CollectNodes(child);
		}
	}

	public void Clear()
	{
		_nodes.Clear();
	}

	// 每帧更新布局
	public void Solve()
	{
		// 初始化所有依赖关系
		foreach (var node in _nodes)
		{
			node.InitializeDependencies();
		}

		//// 重置所有属性状态
		//foreach (var node in _nodes)
		//{
		//	node.Top.Reset();
		//	node.Left.Reset();
		//	node.Width.Reset();
		//	node.Height.Reset();
		//	// 重置其他属性...
		//}

		// 求解布局（可能需要多次遍历）

		foreach (var node in _nodes)
		{
			node.ResolveDependencies();
		}
	}
}
