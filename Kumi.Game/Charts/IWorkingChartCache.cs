namespace Kumi.Game.Charts;

public interface IWorkingChartCache
{
    WorkingChart? GetWorkingChart(ChartInfo chartInfo);

    void Invalidate(ChartSetInfo info);

    void Invalidate(ChartInfo chartInfo);
}
