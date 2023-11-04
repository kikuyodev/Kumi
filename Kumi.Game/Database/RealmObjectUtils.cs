using System.Runtime.Serialization;
using AutoMapper;
using AutoMapper.Internal;
using Kumi.Game.Charts;
using Realms;

namespace Kumi.Game.Database;

public static class RealmObjectUtils
{
    private static readonly IMapper mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<ChartSetInfo, ChartSetInfo>()
           .ConstructUsing(_ => new ChartSetInfo(null))
           .MaxDepth(2)
           .AfterMap((_, d) =>
            {
                foreach (var chart in d.Charts)
                    chart.ChartSet = d;
            });

        c.CreateMap<ChartInfo, ChartInfo>()
           .MaxDepth(2)
           .AfterMap((_, d) =>
            {
                for (var i = 0; i < d.ChartSet?.Charts.Count; i++)
                {
                    if (d.ChartSet.Charts[i].Equals(d))
                    {
                        d.ChartSet.Charts[i] = d;
                        break;
                    }
                }
            });
    }).CreateMapper();

    private static readonly IMapper set_mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<ChartSetInfo, ChartSetInfo>()
           .ConstructUsing(_ => new ChartSetInfo(null))
           .MaxDepth(2)
           .AfterMap((_, d) =>
            {
                foreach (var chart in d.Charts)
                    chart.ChartSet = d;
            });

        c.CreateMap<ChartInfo, ChartInfo>()
           .MaxDepth(1)
           .ForMember(m => m.ChartSet, cc => cc.Ignore());
    }).CreateMapper();
    
    private static void setConfiguration(IMapperConfigurationExpression c)
    {
        c.ShouldMapField = _ => false;
        c.ShouldMapProperty = p => p.GetMethod?.IsPublic == true;
        
        c.Internal().ForAllMaps((_, ex) =>
        {
            ex.ForAllMembers(m =>
            {
                if (m.DestinationMember.Has<IgnoredAttribute>() || m.DestinationMember.Has<BacklinkAttribute>() || m.DestinationMember.Has<IgnoreDataMemberAttribute>())
                    m.Ignore();
            });
        });

        c.CreateMap<ChartInfo, ChartInfo>();
        c.CreateMap<ChartMetadata, ChartMetadata>();
        c.CreateMap<ChartSetInfo, ChartSetInfo>();
    }

    public static List<T> Detach<T>(this IEnumerable<T> items)
        where T : RealmObjectBase
    {
        var list = new List<T>();

        foreach (var obj in items)
            list.Add(obj.Detach());

        return list;
    }
    
    public static T Detach<T>(this T item)
        where T : RealmObjectBase
    {
        if (!item.IsManaged)
            return item;
        
        if (item is ChartSetInfo set)
            return set_mapper.Map<T>(set);

        return mapper.Map<T>(item);
    }
}
