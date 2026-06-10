using System;
using System.Collections.Generic;
using System.Linq;

namespace FlatLife.Mapping;

public abstract class BaseMapper<TSource, TTarget>
{
    public abstract TTarget Map(TSource source);

    public IList<TTarget> MapList(IEnumerable<TSource> sourceEnumerable)
    {
        return sourceEnumerable.Select(Map).ToList();
    }
}
