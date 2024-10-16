using ARWNI2S.Node.Data.Entities.Localization;
using ARWNI2S.Portal.Services.ExportImport.Help;
using ClosedXML.Excel;

namespace ARWNI2S.Portal.Services.ExportImport
{
    public class WorkbookMetadata<T>
    {
        public List<PropertyByName<T, Language>> DefaultProperties { get; set; }

        public List<PropertyByName<T, Language>> LocalizedProperties { get; set; }

        public IXLWorksheet DefaultWorksheet { get; set; }

        public List<IXLWorksheet> LocalizedWorksheets { get; set; }
    }
}
