namespace Sitecore.Support.Pipelines.RenderField
{
  using Sitecore.Diagnostics;
  using System;
  using System.Text;
  using Sitecore.Pipelines.RenderField;

  public class SetAnchorsPositionInLinks : Sitecore.Pipelines.RenderField.SetAnchorsPositionInLinks
  {
    #region Public Methods

    /// <summary>Sets the correct position of anchors in RTE links.</summary>
    /// <param name="args">The arguments.</param>
    /// <contract>
    ///   <requires name="args" condition="none"/>
    /// </contract>
    public virtual new void Process([NotNull] RenderFieldArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (args.FieldTypeKey == "rich text")
      {
        args.Result.FirstPart = this.CheckLinks(args.Result.FirstPart);
        args.Result.LastPart = this.CheckLinks(args.Result.LastPart);
      }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Checks if links exist in the RTE field and modifies them.
    /// </summary>
    /// <param name="text">The text.</param>
    protected virtual new string CheckLinks(string text)
    {
      if (!text.Contains(pattern))
      {
        return text;
      }

      int indexOfPattern = 0;
      int indexOfUrlClosingQuote = 0;
      StringBuilder result = new StringBuilder();
      while ((indexOfPattern = text.IndexOf(pattern, indexOfUrlClosingQuote, StringComparison.Ordinal)) >= 0)
      {
        var lengthOfTextWithoutUrl = indexOfPattern - indexOfUrlClosingQuote + pattern.Length;
        result.Append(text.Substring(indexOfUrlClosingQuote, lengthOfTextWithoutUrl));
        #region Fix 290081 bug
        var lengthOfUrl = text.IndexOf("\"", indexOfPattern + pattern.Length, StringComparison.Ordinal) - indexOfPattern - pattern.Length;
        #endregion
        string urlFromPattern = text.Substring(indexOfPattern + pattern.Length, lengthOfUrl);
        urlFromPattern = this.MoveAnchor(urlFromPattern);
        result.Append(urlFromPattern);
        indexOfUrlClosingQuote = indexOfPattern + pattern.Length + urlFromPattern.Length;
      }

      result.Append(text.Substring(indexOfUrlClosingQuote));

      return result.ToString();
    }
    #endregion
  }
}