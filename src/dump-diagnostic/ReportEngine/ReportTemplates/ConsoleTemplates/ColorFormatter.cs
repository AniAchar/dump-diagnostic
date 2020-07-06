using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.Text;

namespace ReportEngine.ReportTemplates.ConsoleTemplates
{
    internal static class ColorFormatter
    {
        public static TextSpan LightGreen(this string value) =>
            new ContainerSpan(ForegroundColorSpan.LightGreen(),
                              new ContentSpan(value),
                              ForegroundColorSpan.Reset());
        public static TextSpan Underline(this string value) =>
            new ContainerSpan(StyleSpan.UnderlinedOn(),
                              new ContentSpan(value),
                              StyleSpan.UnderlinedOff());

    }
}
