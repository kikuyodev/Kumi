using System.Diagnostics;
using System.Runtime.Serialization;
using AutoMapper;
using AutoMapper.Internal;
using Kumi.Game.Charts;
using Kumi.Game.Models;
using Kumi.Game.Scoring;
using osu.Framework.Logging;
using Realms;

namespace Kumi.Game.Database;

public static class RealmObjectUtils
{
    private static readonly IMapper write_mapper = new MapperConfiguration(c =>
    {
        c.ShouldMapField = _ => false;
        c.ShouldMapProperty = pi => pi.SetMethod?.IsPublic == true;

        c.CreateMap<ChartMetadata, ChartMetadata>()
           .ForMember(s => s.Creator, cc => cc.Ignore())
           .AfterMap((s, d) =>
            {
                copyChangesToRealm(s.Creator, d.Creator);
            });

        c.CreateMap<RealmAccount, RealmAccount>();
        c.CreateMap<RealmFile, RealmFile>();
        c.CreateMap<RealmNamedFileUsage, RealmNamedFileUsage>();

        c.CreateMap<ChartInfo, ChartInfo>()
           .ForMember(s => s.Metadata, cc => cc.Ignore())
           .ForMember(s => s.ChartSet, cc => cc.Ignore())
           .AfterMap((s, d) =>
            {
                copyChangesToRealm(s.Metadata, d.Metadata);
            });

        c.CreateMap<ChartSetInfo, ChartSetInfo>()
           .ConstructUsing(_ => new ChartSetInfo(null))
           .ForMember(s => s.Charts, cc => cc.Ignore())
           .AfterMap((s, d) =>
            {
                foreach (var chart in s.Charts)
                {
                    var existingChart = d.Realm!.Find<ChartInfo>(chart.ID);

                    if (existingChart != null)
                    {
                        if (!d.Charts.Contains(existingChart))
                        {
                            Debug.Fail("Charts should never become detached under normal circumstances.");
                            Logger.Log("Warning: Chart became detached from its ChartSet.", LoggingTarget.Database, LogLevel.Important);
                            d.Charts.Add(existingChart);
                        }
                        
                        copyChangesToRealm(chart, existingChart);
                    }
                    else
                    {
                        var newChart = new ChartInfo
                        {
                            ID = chart.ID,
                            ChartSet = d,
                        };
                        
                        d.Charts.Add(newChart);
                        copyChangesToRealm(chart, newChart);
                    }
                }
            });
        
        c.Internal().ForAllMaps((_, expression) =>
        {
            expression.ForAllMembers(m =>
            {
                if (m.DestinationMember.Has<IgnoreDataMemberAttribute>() || m.DestinationMember.Has<BacklinkAttribute>() || m.DestinationMember.Has<IgnoreDataMemberAttribute>())
                    m.Ignore();
            });
        });
    }).CreateMapper();

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
                    if (d.ChartSet.Charts[i].Equals(d))
                    {
                        d.ChartSet.Charts[i] = d;
                        break;
                    }
            });
    }).CreateMapper();

    private static readonly IMapper set_mapper = new MapperConfiguration(c =>
    {
        setConfiguration(c);

        c.CreateMap<ChartSetInfo, ChartSetInfo>()
           .ConstructUsing(_ => new ChartSetInfo(null))
           .MaxDepth(2)
           .ForMember(c => c.Files, cc => cc.Ignore())
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
        c.CreateMap<ScoreInfo, ScoreInfo>();
        c.CreateMap<RealmAccount, RealmAccount>();
        c.CreateMap<RealmFile, RealmFile>();
        c.CreateMap<RealmNamedFileUsage, RealmNamedFileUsage>();
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

    public static void CopyChangesToRealm(this ChartSetInfo source, ChartSetInfo target)
        => copyChangesToRealm(source, target);

    private static void copyChangesToRealm<T>(T source, T target)
        where T : RealmObjectBase
        => write_mapper.Map(source, target);
}
