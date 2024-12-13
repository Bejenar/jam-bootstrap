using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class StatefulCMS
{
}

[Serializable]
public class StatefulModel
{
    public CMSEntity model;
    public List<StatefulCMS> state = new();

    public StatefulModel(CMSEntity model)
    {
        this.model = model;
    }
    
    public T GetOrAdd<T>() where T : StatefulCMS, new()
    {
        var component = state.OfType<T>().FirstOrDefault();
        if (component == null)
        {
            component = new T();
            state.Add(component);
        }

        return component;
    }
}

[Serializable]
public class StatefulModelContainer
{
    public List<StatefulModel> content = new();

    public StatefulModel GetOrAdd<T>(T model) where T : CMSEntity
    {
        foreach (var ts in content)
        {
            if (ts.model is T)
                return ts;
        }

        var traitState = new StatefulModel(model);
        content.Add(traitState);
        return traitState;
    }

    public bool Is<T>(out T t) where T : EntityComponentDefinition, new()
    {
        foreach (var c in content)
        {
            if (c.model.Is(out t))
                return true;
        }

        t = null;
        return false;
    }

    public bool IsWithState<Tag, State>(out State outState)
        where State : StatefulCMS, new()
        where Tag : EntityComponentDefinition, new()
    {
        foreach (var c in content)
        {
            if (c.model.Is<Tag>(out _) && c.GetOrAdd<State>() is State s)
            {
                outState = s;
                return true;
            }
        }

        outState = null;
        return false;
    }

    public List<T> GetAll<T>() where T : EntityComponentDefinition, new()
    {
        var allSearch = new List<T>();

        foreach (var a in content)
            if (a.model.Is<T>(out var t))
                allSearch.Add(t);

        return allSearch;
    }

    public void Remove<T>() where T : EntityComponentDefinition, new()
    {
        content.RemoveAll(c => c.model.Is<T>(out _));
    }

    public void RemoveAll()
    {
        content.Clear();
    }

    public StatefulModelContainer Clone()
    {
        return new StatefulModelContainer
        {
            content = new List<StatefulModel>(content)
        };
    }
}