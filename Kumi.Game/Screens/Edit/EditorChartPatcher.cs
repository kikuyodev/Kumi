using System.Collections.Immutable;
using System.Text;
using DiffPlex;
using DiffPlex.Model;
using Kumi.Game.Charts;
using Kumi.Game.Charts.Formats;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Charts.Timings;
using Kumi.Game.Extensions;
using Kumi.Game.IO.Formats;

namespace Kumi.Game.Screens.Edit;

public class EditorChartPatcher
{
    private readonly EditorChart editorChart;
    private readonly ChartDecoder decoder = new ChartDecoder();

    public EditorChartPatcher(EditorChart editorChart)
    {
        this.editorChart = editorChart;
    }

    public void Patch(byte[] currentState, byte[] newState)
    {
        var difference = new Differ().CreateLineDiffs(readString(currentState), readString(newState), true, false);
        IChart? newChart = null;
        
        editorChart.BeginChange();
        processNotes(difference, () => newChart ??= readChart(newState));
        processTimingPoints(() => newChart ??= readChart(newState));
        editorChart.EndChange();
    }

    private void processTimingPoints(Func<IChart> getNewChart)
    {
        var newTimingPoints = getNewChart().TimingPoints;
        var oldTimingPoints = editorChart.TimingPoints.ToImmutableList();

        foreach (var point in oldTimingPoints)
        {
            if (!newTimingPoints.Contains(point))
                editorChart.TimingPointHandler.TimingPoints.Remove((TimingPoint) point);
        }
        
        foreach (var point in newTimingPoints)
        {
            if (!editorChart.TimingPoints.Contains(point))
                editorChart.TimingPointHandler.TimingPoints.Add((TimingPoint) point);
        }
    }

    private void processNotes(DiffResult result, Func<IChart> getNewChart)
    {
        findChangedIndices(result, ChartSections.Notes, out var removedIndices, out var addedIndices);

        for (var i = removedIndices.Count - 1; i >= 0; i--)
            editorChart.RemoveAt(removedIndices[i]);

        if (addedIndices.Count > 0)
        {
            var newChart = getNewChart();

            foreach (var i in addedIndices)
                editorChart.Insert(i, (Note)newChart.Notes[i]);
        }
    }

    private void findChangedIndices(DiffResult result, ChartSections section, out List<int> removedIndices, out List<int> addedIndices)
    {
        removedIndices = new List<int>();
        addedIndices = new List<int>();
        
        var sectionHeader = ((IFileHandler<Chart, ChartSections>) decoder).SectionHeader;

        var oldSectionStartIndex = Array.IndexOf(result.PiecesOld, getSectionHeader(section));
        if (oldSectionStartIndex == -1)
            return;

        var oldSectionEndIndex = Array.FindIndex(result.PiecesOld, oldSectionStartIndex + 1, s => s.StartsWith(sectionHeader.Start));
        if (oldSectionEndIndex == -1)
            oldSectionEndIndex = result.PiecesOld.Length;
        
        var newSectionStartIndex = Array.IndexOf(result.PiecesNew, getSectionHeader(section));
        if (newSectionStartIndex == -1)
            return;
        
        var newSectionEndIndex = Array.FindIndex(result.PiecesNew, newSectionStartIndex + 1, s => s.StartsWith(sectionHeader.Start));
        if (newSectionEndIndex == -1)
            newSectionEndIndex = result.PiecesNew.Length;

        foreach (var block in result.DiffBlocks)
        {
            // Removed indices
            for (var i = 0; i < block.DeleteCountA; i++)
            {
                var objectIndex = block.DeleteStartA + i;
                
                if (objectIndex <= oldSectionStartIndex || objectIndex >= oldSectionEndIndex)
                    continue;
                
                removedIndices.Add(objectIndex - oldSectionStartIndex - 1);
            }
            
            // Added indices
            for (var i = 0; i < block.InsertCountB; i++)
            {
                var objectIndex = block.InsertStartB + i;
                
                if (objectIndex <= newSectionStartIndex || objectIndex >= newSectionEndIndex)
                    continue;
                
                addedIndices.Add(objectIndex - newSectionStartIndex - 1);
            }
        }
        
        removedIndices.Sort();
        addedIndices.Sort();
    }
    
    private string getSectionHeader(ChartSections section)
    {
        var sectionHeader = ((IFileHandler<Chart, ChartSections>) decoder).SectionHeader;
        return $"{sectionHeader.Start}{section.ToString().ToScreamingSnakeCase()}{sectionHeader.End}";
    }

    private string readString(byte[] bytes)
        => Encoding.UTF8.GetString(bytes);

    private IChart readChart(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return decoder.Decode(stream);
    }
}
