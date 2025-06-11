using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luxcinder.Functions.UISystem.UICore;

public enum DependencyState
{
    Unresolved,
    Resolving,
    Resolved,
    Invalid
}

public interface IDependencyProperty
{
    object Value { get; set;}

	/// <summary>
	/// Gets a value indicating whether the property has been resolved.
	/// </summary>
	DependencyState GetDependencyState
    {
        get;
    }

    /// <summary>
    /// Ensures that the property's value is resolved.
    /// This method will trigger the resolution process if not already resolved.
    /// </summary>
    void Resolve();

    // void AddDependee(IDependencyProperty dependency);

	public void ResetDependencyState();
}

public class DependencyProperty<T> : IDependencyProperty
{
    private T _value;
    private Func<object[], T> _valueResolver;
    private List<IDependencyProperty> _dependencies = new List<IDependencyProperty>();
    private List<IDependencyProperty> _dependees = new List<IDependencyProperty>();
    private DependencyState _dependencyState = DependencyState.Unresolved;

	public object Value
    {
        get => _value;
        set => _value = (T)value;
    }

    public T TypedValue
    {
        get => _value;
        set => _value = value;
    }

    public DependencyState GetDependencyState
	{
        get => _dependencyState;
    }

	public void ResetDependencyState()
	{
		_dependencyState = DependencyState.Unresolved;
	}

	public void AddDependee(IDependencyProperty dependency) => _dependees.Add(dependency);

    /// <summary>
    /// Binds this dependency property to a resolver function and its dependencies.
    /// </summary>
    /// <param name="resolver">The function that resolves the value based on the dependencies.</param>
    /// <param name="dependencies">The list of dependencies that this property relies on.</param>

    public void Bind(Func<object[], T> resolver, List<IDependencyProperty> dependencies)
    {
		ResetDependencyState();
		_valueResolver = resolver;
        _dependencies = dependencies ?? new List<IDependencyProperty>();
        //foreach(var dep in _dependencies)
        //{
        //    dep.AddDependee(this);
        //}
        if (_dependencies.Count == 0)
        {
            _dependencyState = DependencyState.Resolved;
        }
    }

    public void Bind(Func<T, T> resolver, IDependencyProperty dependency)
    {
		ResetDependencyState();
        _valueResolver = (os) => resolver((T)os[0]);
        _dependencies = new List<IDependencyProperty>() { dependency };

        //dependency.AddDependee(this);
    }

	public void Bind(Func<T> resolver)
	{
		ResetDependencyState();
		_valueResolver = (os) => resolver();
		_dependencies = new List<IDependencyProperty>();
		_dependencyState = DependencyState.Resolved; // No dependencies, so it's resolved immediately.
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

            object[] depValues = new object[_dependencies.Count];
            for (int i = 0; i < _dependencies.Count; i++)
            {
                // Will recursively call Resolve on dependencies.
                // If a cycle is formed, the _isResolving flag in one of the ancestors will catch it.
                _dependencies[i].Resolve();
                depValues[i] = _dependencies[i].Value;
            }

            _value = _valueResolver(depValues);
            _dependencyState = DependencyState.Resolved;
        }
        finally
        {
            _dependencyState = DependencyState.Invalid;
        }
    }
}