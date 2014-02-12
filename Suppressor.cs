using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.JavaScript.Web.Extensions;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Web.Editor;
using Microsoft.Web.Editor;

namespace SLaks.RefSyncKiller {
    class Suppressor : IDisposable {
        // Copied from Microsoft.VisualStudio.JavaScript.Web.Extensions.ReferenceAutoSync.ProjectServices

        [Import]
        private IWebProjectServices _webProjectServices;

        private static readonly Type ReferenceSyncManager = typeof(JavaScriptWebExtensionsPackage).Assembly.GetType("Microsoft.VisualStudio.JavaScript.Web.Extensions.ReferenceAutoSync.ReferenceSyncManager");

        internal Suppressor() {
            if (ReferenceSyncManager == null)
                return;

            WebEditor.CompositionService.SatisfyImportsOnce(this);
            this.Initialize();
        }
        private void Initialize() {
            foreach (IVsHierarchy current in this._webProjectServices.OpenedProjects) {
                this.InitializeProject(current);
            }
            this._webProjectServices.ProjectOpened += OnWebProjectOpened;
            //this._webProjectServices.ProjectClosing += OnWebProjectClosing;
        }
        private void OnWebProjectOpened(object sender, ProjectEventArgs e) {
            this.InitializeProject(e.Project);
        }
        private void InitializeProject(IVsHierarchy webProject) {
            WebProjectData projectData = this._webProjectServices.GetProjectData(webProject);
            projectData.Properties.AddProperty(ReferenceSyncManager, FormatterServices.GetSafeUninitializedObject(ReferenceSyncManager));
        }
        // We don't need to dispose our fake objects
        //private void OnWebProjectClosing(object sender, ProjectEventArgs e) {
        //    this.DisposeProject(e.Project);
        //}
        //private void DisposeProject(IVsHierarchy webProject) {
        //    WebProjectData projectData = this._webProjectServices.GetProjectData(webProject);
        //    ReferenceSyncManager property = projectData.Properties.GetProperty<ReferenceSyncManager>(typeof(Microsoft.VisualStudio.JavaScript.Web.Extensions.ReferenceAutoSync.ReferenceSyncManager));
        //    property.Dispose();
        //}
        public void Dispose() {
            //foreach (IVsHierarchy current in this._webProjectServices.OpenedProjects) {
            //    this.DisposeProject(current);
            //}
            this._webProjectServices.ProjectOpened -= OnWebProjectOpened;
            //this._webProjectServices.ProjectClosing -= OnWebProjectClosing;
        }
    }
}
