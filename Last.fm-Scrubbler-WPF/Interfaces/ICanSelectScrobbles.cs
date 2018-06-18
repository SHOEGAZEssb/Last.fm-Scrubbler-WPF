using System.Collections.ObjectModel;

namespace Scrubbler.Interfaces
{
  interface ICanSelectScrobbles<T> where T : IScrobbableObject
  {
    ObservableCollection<T> Scrobbles { get; }
    void SelectAll();
    void SelectNone();
    bool CanSelectAll { get; }
    bool CanSelectNone { get; }
  }
}