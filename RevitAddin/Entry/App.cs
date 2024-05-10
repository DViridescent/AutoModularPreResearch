using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIFramework;

namespace ArchiAlpha.AutoModular.PreResearch.Revit.Entry
{
    internal class App : IExternalApplication
    {
        private const string TAB_NAME = "工业化技术预研";
        public Result OnStartup(UIControlledApplication application)
        {
            if (RevitRibbonControl.RibbonControl.FindTab(TAB_NAME) == null)
            {
                application.CreateRibbonTab(TAB_NAME);
            }

            var viewPanel = application.CreateRibbonPanel(TAB_NAME, "视图和视图样板");
            var workSetPanel = application.CreateRibbonPanel(TAB_NAME, "对象和工作集");

            // 主界面按钮
            viewPanel.AddItem(new PushButtonData("CreateView", "创建视图", typeof(App).Assembly.Location, typeof(CreateViewCommand).FullName));
            workSetPanel.AddItem(new PushButtonData("CreateWorkset", "创建工作集", typeof(App).Assembly.Location, typeof(CreateWorksetCommand).FullName));
            workSetPanel.AddItem(new PushButtonData("CreateObjects", "生成对象", typeof(App).Assembly.Location, typeof(CreateObjectsCommand).FullName));

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;
    }
}
