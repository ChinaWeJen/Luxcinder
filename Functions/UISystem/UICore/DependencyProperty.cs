using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace Luxcinder.Functions.UISystem.UICore;

public enum DependencyState
{
    Unresolved,
    Resolving,
    Resolved,
    Invalid
}

public class DependencyProperty<T>
{
    private T _value;
    private Func<T> _valueResolver;
    private DependencyState _dependencyState = DependencyState.Invalid;

	public object Value
    {
        get
        {
            if (_dependencyState != DependencyState.Resolved)
                Resolve();

            return _value;
        }
        set => _value = (T)value;
    }

    public T TypedValue
    {
        get => (T)Value;
        set => Value = value;
    }

    public DependencyState GetDependencyState
	{
        get => _dependencyState;
    }

	public void ResetDependencyState()
	{
		_dependencyState = DependencyState.Unresolved;
	}

	////public void AddDependee(IDependencyProperty dependency) => _dependees.Add(dependency);

 //   /// <summary>
 //   /// Binds this dependency property to a resolver function and its dependencies.
 //   /// </summary>
 //   /// <param name="resolver">The function that resolves the value based on the dependencies.</param>
 //   /// <param name="dependencies">The list of dependencies that this property relies on.</param>

 //   public void Bind(Func<object[], T> resolver, List<> dependencies)
 //   {
	//	ResetDependencyState();
	//	_valueResolver = resolver;
 //       _dependencies = dependencies ?? new List<IDependencyProperty>();
 //       //foreach(var dep in _dependencies)
 //       //{
 //       //    dep.AddDependee(this);
 //       //}
 //       if (_dependencies.Count == 0)
 //       {
 //           _dependencyState = DependencyState.Resolved;
 //       }
 //   }

 //   public void Bind(Func<T, T> resolver, IDependencyProperty dependency)
 //   {
	//	ResetDependencyState();
 //       _valueResolver = (os) => resolver((T)os[0]);
 //       _dependencies = new List<IDependencyProperty>() { dependency };

 //       //dependency.AddDependee(this);
 //   }

	public void Bind(Func<T> resolver)
	{
		ResetDependencyState();
		_valueResolver = resolver;
	}

	public void Resolve()
    {
        if (_dependencyState == DependencyState.Resolved)
        {
            return;
        }

        if (_dependencyState == DependencyState.Resolving)
        {
            throw new InvalidOperationException("Circular dependency detected.");
        }

        _dependencyState = DependencyState.Resolving;
        try
        {
            if (_valueResolver == null)
            {
                // If there's no resolver, it means it was default-constructed and never bound,
                // or its value was set directly (which would set _isResolved = true, handled by the first check).
                // In the case of default-construction and no binding, it's considered resolved with its current value (default(T)).
                _dependencyState = DependencyState.Resolved; // _value is already default(T)
                return;
            }

            _value = _valueResolver();
            _dependencyState = DependencyState.Resolved;
        }
        finally
        {
            _dependencyState = DependencyState.Invalid;
        }
    }
}