namespace Scrubbler.ViewModels.ExtraFunctions
{
  /// <summary>
  /// Base class for all extra function ViewModels.
  /// </summary>
  public abstract class ExtraFunctionViewModelBase : ViewModelBase
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="displayName">Display name.</param>
    public ExtraFunctionViewModelBase(string displayName)
    {
      DisplayName = displayName;
    }
  }
}